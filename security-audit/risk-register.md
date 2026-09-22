# ArrayApp Platform — Security Risk Register

**Audit Date**: September 22, 2026  
**Methodology**: OWASP Top 10 (2021), OWASP API Security Top 10 (2023), CWE/SANS Top 25, CVSS v3.1  
**Scope**: Repository `ArrayApp` (.NET 10, ASP.NET Core WebAPI, Infrastructure, Domain, CI/CD Workflows, Containers)  

---

## Severity Breakdown Summary

| Severity | Total Discovered | Resolved in Audit | Unresolved / Operational Action Required |
| :--- | :---: | :---: | :---: |
| 🔴 **Critical** | 3 | 3 | 0 |
| 🟠 **High** | 5 | 5 | 0 |
| 🟡 **Medium** | 4 | 4 | 0 |
| 🔵 **Low** | 3 | 2 | 1 (External Secrets Vault Integration) |
| ⚪ **Informational** | 2 | 2 | 0 |
| **Total** | **17** | **16** | **1** |

---

## Detailed Findings Matrix

### SEC-001: Broken Object Level Authorization (BOLA) Leading to Account Takeover
- **Title**: Arbitrary Account Takeover via Unverified Password Reset
- **Category**: Broken Access Control / Insecure Direct Object Reference (IDOR / BOLA)
- **Affected Component**: Identity & Authentication Subsystem
- **Affected File**: `src/ArrayApp.WebAPI/Controllers/AccountController.cs` (lines 300–324)
- **CWE**: [CWE-639](https://cwe.mitre.org/data/definitions/639.html) (Authorization Bypass Through User-Controlled Key), [CWE-284](https://cwe.mitre.org/data/definitions/284.html) (Improper Access Control)
- **OWASP**: API1:2023 Broken Object Level Authorization, A01:2021 Broken Access Control
- **CVSS v3.1 Base Score**: **9.8** (`CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:H/A:H`)
- **Severity**: 🔴 **Critical**
- **Description**: The `ResetPassword` endpoint accepted an arbitrary `userId` and `newPassword` in the request body. Upon receipt, it called `_userManager.GeneratePasswordResetTokenAsync(user)` internally and immediately reset the user's password without verifying any token supplied by the caller, without checking the caller's identity, and without requiring the current password.
- **Attack Scenario**: An unauthenticated or low-privilege attacker discovers the `userId` of an administrator or executive via user enumeration and posts `{ "userId": "1", "newPassword": "HackedPassword123!" }` to `/api/Account/ResetPassword`. The server generates the reset token internally and resets the administrator's password, granting the attacker complete control of the administrator account.
- **Evidence**:
  ```csharp
  // Pre-audit vulnerable implementation
  var user = await _userManager.FindByIdAsync(model.UserId);
  var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
  var result = await _userManager.ResetPasswordAsync(user, resetToken, model.NewPassword);
  ```
- **Remediation**: Implemented strict caller ownership verification (`currentUserId == model.UserId` unless caller holds `Administrator` role); required callers to supply a valid `ResetToken` (generated through OTP flow) or their `CurrentPassword`; prohibited automatic internal token generation for arbitrary users.
- **Remediation Status**: ✅ **Fixed**
- **Verification Status**: ✅ Verified with automated unit tests in `tests/Security.UnitTests/AccountSecurityUnitTests.cs`.

---

### SEC-002: Vertical Privilege Escalation via Unrestricted Role Management APIs
- **Title**: Low-Privilege Users Able to Grant Themselves Administrator Role
- **Category**: Broken Access Control / Elevation of Privilege
- **Affected Component**: User Management Subsystem
- **Affected File**: `src/ArrayApp.WebAPI/Controllers/AccountController.cs` (lines 266–298, 372–445)
- **CWE**: [CWE-269](https://cwe.mitre.org/data/definitions/269.html) (Improper Privilege Management), [CWE-285](https://cwe.mitre.org/data/definitions/285.html) (Improper Authorization)
- **OWASP**: API5:2023 Broken Function Level Authorization, A01:2021 Broken Access Control
- **CVSS v3.1 Base Score**: **8.8** (`CVSS:3.1/AV:N/AC:L/PR:L/UI:N/S:U/C:H/I:H/A:H`)
- **Severity**: 🔴 **Critical**
- **Description**: Endpoints `ResetRoles`, `AddRolesToUser`, `RemoveRolesFromUser`, `CreateRole`, and `DeleteRole` were decorated with class-level `[Authorize]` but lacked role-based access restrictions (`[Authorize(Roles = "Administrator")]`). Any authenticated standard user could invoke these endpoints to grant themselves or other accounts administrative roles.
- **Attack Scenario**: A newly registered user with no privileges sends an HTTP POST request to `/api/Account/AddRolesToUser` with `{ "userId": "<attacker-id>", "roleNames": ["Administrator"] }`. The request succeeded, immediately elevating the caller to full administrative control.
- **Evidence**:
  ```csharp
  // Pre-audit vulnerable implementation: No role restriction on role modification
  [HttpPost]
  [Route("AddRolesToUser")]
  public async Task<ServiceResponse<bool>> AddRolesToUser(AddRolesToUserModel model)
  ```
- **Remediation**: Decorated all role creation, deletion, assignment, and reset endpoints with `[Authorize(Roles = "Administrator,Admin,admin")]`.
- **Remediation Status**: ✅ **Fixed**
- **Verification Status**: ✅ Verified with automated unit tests in `tests/Security.UnitTests/AccountSecurityUnitTests.cs`.

---

### SEC-003: Sub-256-Bit Cryptographic Key and Disabled Token Validation
- **Title**: Insecure JWT Validation Configuration & Insufficient Key Entropy
- **Category**: Cryptographic Failures
- **Affected Component**: Authentication Middleware
- **Affected File**: `src/ArrayApp.WebAPI/Program.cs` (lines 65–88), `appsettings.json` (line 23)
- **CWE**: [CWE-326](https://cwe.mitre.org/data/definitions/326.html) (Inadequate Encryption Strength), [CWE-347](https://cwe.mitre.org/data/definitions/347.html) (Improper Verification of Cryptographic Signature)
- **OWASP**: API2:2023 Broken Authentication, A02:2021 Cryptographic Failures
- **CVSS v3.1 Base Score**: **9.1** (`CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:H/A:N`)
- **Severity**: 🔴 **Critical**
- **Description**: In `Program.cs`, token validation parameters explicitly disabled issuer and audience validation (`ValidateIssuer = false`, `ValidateAudience = false`, `RequireHttpsMetadata = false`). Furthermore, `appsettings.json` contained a 16-character (128-bit) symmetric key `"C421AAEE0D114E9C"`, directly violating RFC 7518 Section 3.2 which mandates a minimum key size of 256 bits (32 bytes) for HMAC-SHA256.
- **Attack Scenario**: Because `ValidateIssuer` and `ValidateAudience` were false, an attacker could present a JWT issued by an untrusted foreign identity provider or a token generated with a dictionary-cracked 128-bit secret to authenticate as any user.
- **Evidence**:
  ```csharp
  // Pre-audit vulnerable implementation
  ValidateIssuer = false,
  ValidateAudience = false,
  RequireHttpsMetadata = false
  ```
- **Remediation**: Enforced `ValidateIssuer = true`, `ValidateAudience = true`, `ValidateLifetime = true`, and `RequireHttpsMetadata = true` (in non-development). Enforced minimum 256-bit key length and updated configuration with a secure key placeholder.
- **Remediation Status**: ✅ **Fixed**
- **Verification Status**: ✅ Verified with automated unit tests in `tests/Security.UnitTests/JwtSecurityConfigurationUnitTests.cs`.

---

### SEC-004: Unauthenticated Public Exposure of 35 Business API Controllers
- **Title**: Widespread Missing Authentication Across Application Controllers
- **Category**: Broken Access Control
- **Affected Component**: API Routing & Controller Architecture
- **Affected File**: Multiple controllers in `src/ArrayApp.WebAPI/Controllers/`
- **CWE**: [CWE-306](https://cwe.mitre.org/data/definitions/306.html) (Missing Authentication for Critical Function)
- **OWASP**: API2:2023 Broken Authentication, A01:2021 Broken Access Control
- **CVSS v3.1 Base Score**: **8.6** (`CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:L/A:L`)
- **Severity**: 🟠 **High**
- **Description**: 35 out of 38 API controllers lacked the `[Authorize]` attribute entirely. Endpoints handling AI agents, connectors, file uploads, subscriptions, campaigns, and governance were directly callable by anonymous internet actors.
- **Attack Scenario**: An anonymous attacker connects to `/api/ZeroTrustGovernance/evaluate-access/1`, `/api/Connectors/1`, or `/api/AIAgents/invoke` and invokes internal enterprise logic without providing credentials.
- **Remediation**: Added `[Authorize]` across all API controllers; explicitly marked intended public endpoints (`/Token`, `/Account/SignUp`, `/Account/SendResetOTP`) with `[AllowAnonymous]`.
- **Remediation Status**: ✅ **Fixed**
- **Verification Status**: ✅ Verified with automated reflection audit test in `tests/Security.UnitTests/ControllerAuthorizationAuditUnitTests.cs`.

---

### SEC-005: Sensitive Data & Credential Hash Disclosure in User APIs
- **Title**: User Query Endpoints Return Raw ApplicationUser EF Entities with Password Hashes
- **Category**: Excessive Data Exposure / Sensitive Data Leakage
- **Affected Component**: User Management Subsystem
- **Affected File**: `src/ArrayApp.WebAPI/Controllers/AccountController.cs` (lines 178–244)
- **CWE**: [CWE-200](https://cwe.mitre.org/data/definitions/200.html) (Exposure of Sensitive Information to an Unauthorized Actor)
- **OWASP**: API3:2023 Broken Object Property Level Authorization, A04:2021 Insecure Design
- **CVSS v3.1 Base Score**: **7.5** (`CVSS:3.1/AV:N/AC:L/PR:L/UI:N/S:U/C:H/I:N/A:N`)
- **Severity**: 🟠 **High**
- **Description**: `GetAllUsers`, `GetUserById`, `GetUserByEmail`, and `GetUserByUsername` returned raw `ApplicationUser` entities directly. These entities serialized `PasswordHash`, `SecurityStamp`, `ConcurrencyStamp`, and contact information in JSON responses.
- **Attack Scenario**: Any authenticated user calls `GET /api/Account/GetAllUsers` and obtains password hashes and security stamps for all registered accounts, enabling offline GPU hash cracking.
- **Evidence**:
  ```csharp
  // Pre-audit vulnerable implementation
  public async Task<IServiceResponse<List<ApplicationUser>>> GetAllUsers()
  ```
- **Remediation**: Created `UserDto` model omitting `PasswordHash` and `SecurityStamp`; updated user endpoints to project into `UserDto`. Added role authorization (`Administrator,Manager`) to `GetAllUsers`.
- **Remediation Status**: ✅ **Fixed**
- **Verification Status**: ✅ Verified with unit tests in `tests/Security.UnitTests/DataSanitizationUnitTests.cs`.

---

### SEC-006: Stack Trace & Internal System Architecture Information Leakage
- **Title**: Raw Exception Stack Traces Returned to Client in API Error Responses
- **Category**: Information Disclosure
- **Affected Component**: Error Handling Middleware & Controllers
- **Affected File**: `src/ArrayApp.WebAPI/Controllers/IdeaController.cs` (29 locations), `src/ArrayApp.WebAPI/Filters/ApiExceptionFilterAttribute.cs`
- **CWE**: [CWE-209](https://cwe.mitre.org/data/definitions/209.html) (Generation of Error Message Containing Sensitive Information)
- **OWASP**: A05:2021 Security Misconfiguration
- **CVSS v3.1 Base Score**: **7.1** (`CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:L/I:N/A:L`)
- **Severity**: 🟠 **High**
- **Description**: 29 `catch` blocks in `IdeaController.cs` and `FileDataController.cs` populated `ApiResponse.Description = ex.StackTrace`. Furthermore, `ApiExceptionFilterAttribute` had no unhandled exception fallback, leaking raw runtime exceptions.
- **Attack Scenario**: An attacker sends a malformed request to `/api/Idea/feed` or `/api/FileData/get/invalid`. The application returns the complete stack trace including full server directory paths, namespace structures, and line numbers.
- **Evidence**:
  ```csharp
  // Pre-audit vulnerable implementation
  catch (Exception ex)
  {
      return BadRequest(new ApiResponse<string> {
          Code = SystemCodes.Failed,
          Data = ex.Message,
          Description = ex.StackTrace
      });
  }
  ```
- **Remediation**: Replaced `ex.StackTrace` across all controller catch blocks with generic sanitized user messages. Enhanced `ApiExceptionFilterAttribute` with `HandleUnknownException` returning RFC 7807 ProblemDetails with opaque correlation `traceId`.
- **Remediation Status**: ✅ **Fixed**
- **Verification Status**: ✅ Verified with unit tests in `tests/Security.UnitTests/ErrorHandlingSecurityUnitTests.cs`.

---

### SEC-007: Zero Trust Attribute-Based Access Control Spoofing
- **Title**: Clearance and Department Parameter Manipulation in Zero Trust ABAC
- **Category**: Broken Access Control
- **Affected Component**: Zero Trust Governance Subsystem
- **Affected File**: `src/ArrayApp.WebAPI/Controllers/ZeroTrustGovernanceController.cs` (lines 23–36)
- **CWE**: [CWE-285](https://cwe.mitre.org/data/definitions/285.html) (Improper Authorization)
- **OWASP**: API1:2023 Broken Object Level Authorization
- **CVSS v3.1 Base Score**: **8.2** (`CVSS:3.1/AV:N/AC:L/PR:L/UI:N/S:U/C:H/I:H/A:N`)
- **Severity**: 🟠 **High**
- **Description**: `EvaluateAccess` permitted the caller to supply `department` and `clearance` via URL query strings rather than extracting them from the authenticated caller's cryptographically verified token claims.
- **Attack Scenario**: A contractor with "Internal" clearance accesses a top-secret idea by appending `?clearance=TopSecret&department=Executive` to the access evaluation request.
- **Remediation**: Bound department and clearance attributes to verified JWT identity claims (`User.FindFirst("Department")`, `User.FindFirst("Clearance")`); enforced `[Authorize]` on the controller.
- **Remediation Status**: ✅ **Fixed**
- **Verification Status**: ✅ Verified with code analysis and solution test suite.

---

### SEC-008: End-of-Life Base Images & Root Execution in Container
- **Title**: Dockerfile Uses EOL .NET 7 Base Images and Runs as Root
- **Category**: Container & Supply-Chain Security
- **Affected Component**: Container Infrastructure
- **Affected File**: `src/ArrayApp.WebAPI/Dockerfile`
- **CWE**: [CWE-250](https://cwe.mitre.org/data/definitions/250.html) (Execution with Unnecessary Privileges), [CWE-1104](https://cwe.mitre.org/data/definitions/1104.html) (Use of Unmaintained Third Party Component)
- **OWASP**: A06:2021 Vulnerable and Outdated Components
- **CVSS v3.1 Base Score**: **7.8** (`CVSS:3.1/AV:L/AC:L/PR:L/UI:N/S:C/C:H/I:H/A:H`)
- **Severity**: 🟠 **High**
- **Description**: `Dockerfile` referenced `mcr.microsoft.com/dotnet/aspnet:7.0` and `sdk:7.0` (EOL May 2024, unpatched CVEs) for an application targeting .NET 10. The container lacked a non-root `USER` declaration, executing as root (UID 0).
- **Attack Scenario**: In the event of a remote code execution vulnerability, the process executes with root privileges inside the container, facilitating container escape and host resource compromise.
- **Remediation**: Upgraded base image to `mcr.microsoft.com/dotnet/aspnet:10.0`; added unprivileged system user `appuser:appgroup` (UID 10001); exposed unprivileged port 8080; added container `HEALTHCHECK`.
- **Remediation Status**: ✅ **Fixed**
- **Verification Status**: ✅ Verified via Dockerfile review and linting.

---

### SEC-009: Hardcoded SA Database Passwords in CI/CD Workflows
- **Title**: Hardcoded SQL Administrator Credentials in GitHub Actions
- **Category**: Secrets Management
- **Affected Component**: CI/CD Pipelines
- **Affected File**: `.github/workflows/dotnet-deploy.yml` (lines 21, 35), `.github/workflows/dotnet-build.yml` (lines 17, 31)
- **CWE**: [CWE-798](https://cwe.mitre.org/data/definitions/798.html) (Use of Hard-coded Credentials)
- **OWASP**: A07:2021 Identification and Authentication Failures
- **CVSS v3.1 Base Score**: **6.5** (`CVSS:3.1/AV:N/AC:L/PR:H/UI:N/S:U/C:H/I:H/A:N`)
- **Severity**: 🟡 **Medium**
- **Description**: CI/CD workflows contained hardcoded SA passwords (`SA_PASSWORD: Your_password123`) and connection strings in plain text.
- **Attack Scenario**: An attacker with read access to the repository observes the SA credentials and uses them to access exposed test database infrastructure.
- **Remediation**: Replaced hardcoded values with GitHub Secrets (`${{ secrets.SQL_SA_PASSWORD }}`).
- **Remediation Status**: ✅ **Fixed**
- **Verification Status**: ✅ Verified via workflow configuration inspection.

---

### SEC-010: Missing HTTP Security Response Headers
- **Title**: Application Lacks Clickjacking, MIME-Sniffing, and CSP Defenses
- **Category**: Security Misconfiguration
- **Affected Component**: HTTP Pipeline
- **Affected File**: `src/ArrayApp.WebAPI/Program.cs`
- **CWE**: [CWE-1021](https://cwe.mitre.org/data/definitions/1021.html) (Improper Restriction of Rendered UI Layers or Frames)
- **OWASP**: A05:2021 Security Misconfiguration
- **CVSS v3.1 Base Score**: **5.4** (`CVSS:3.1/AV:N/AC:L/PR:N/UI:R/S:U/C:L/I:L/A:N`)
- **Severity**: 🟡 **Medium**
- **Description**: The HTTP pipeline did not emit `X-Content-Type-Options`, `X-Frame-Options`, `Referrer-Policy`, or `Content-Security-Policy` headers.
- **Attack Scenario**: A malicious website frames ArrayApp in an invisible iframe, conducting clickjacking attacks to trick users into executing actions on canvas or deleting ideas.
- **Remediation**: Added custom Security Response Headers middleware setting `X-Frame-Options: DENY`, `X-Content-Type-Options: nosniff`, strict CSP, and Referrer-Policy.
- **Remediation Status**: ✅ **Fixed**
- **Verification Status**: ✅ Verified via middleware pipeline review.

---

### SEC-011: Missing Rate Limiting on Authentication and API Endpoints
- **Title**: Absence of Request Throttling Enables Credential Stuffing and DoS
- **Category**: Unrestricted Resource Consumption
- **Affected Component**: HTTP Pipeline / Rate Limiting
- **Affected File**: `src/ArrayApp.WebAPI/Program.cs`
- **CWE**: [CWE-307](https://cwe.mitre.org/data/definitions/307.html) (Improper Restriction of Excessive Authentication Attempts)
- **OWASP**: API4:2023 Unrestricted Resource Consumption
- **CVSS v3.1 Base Score**: **5.3** (`CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:N/I:N/A:L`)
- **Severity**: 🟡 **Medium**
- **Description**: Neither the login endpoint (`/Token`) nor the general API surface had rate limiting configured.
- **Attack Scenario**: An attacker launches automated brute-force attacks against user passwords or floods the canvas sync hub with requests.
- **Remediation**: Configured ASP.NET Core `AddRateLimiter` with a fixed-window partition limiter (120 req/min/identity) returning HTTP 429 Too Many Requests.
- **Remediation Status**: ✅ **Fixed**
- **Verification Status**: ✅ Verified via pipeline review.

---

### SEC-012: Weak Identity Password Policy and Disabled Account Lockout
- **Title**: ASP.NET Identity Configured with Trivial Password Requirements
- **Category**: Identification and Authentication Failures
- **Affected Component**: ASP.NET Identity Configuration
- **Affected File**: `src/Infrastructure/ConfigureServices.cs` (lines 47–62)
- **CWE**: [CWE-521](https://cwe.mitre.org/data/definitions/521.html) (Weak Password Requirements), [CWE-307](https://cwe.mitre.org/data/definitions/307.html)
- **OWASP**: A07:2021 Identification and Authentication Failures
- **CVSS v3.1 Base Score**: **6.5** (`CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:L/I:L/A:N`)
- **Severity**: 🟡 **Medium**
- **Description**: Password complexity was explicitly disabled (`RequireDigit = false`, `RequireUppercase = false`, `RequiredLength = 6`, `Lockout.AllowedForNewUsers = false`).
- **Attack Scenario**: Users register with weak 6-character passwords (e.g. `123456`). Because account lockout was disabled, attackers could brute force credentials without detection.
- **Remediation**: Enforced minimum length 8, digit, uppercase, lowercase, special character requirements, and enabled lockout protection (`MaxFailedAccessAttempts = 5`, `DefaultLockoutTimeSpan = 15m`).
- **Remediation Status**: ✅ **Fixed**
- **Verification Status**: ✅ Verified via configuration review and solution tests.

---

### SEC-013: TokenController Emptied Response and Dropped Role Claims
- **Title**: Functional Logic Error in Token Response and Role Claim Emission
- **Category**: Broken Authentication
- **Affected Component**: Token Service Subsystem
- **Affected File**: `src/ArrayApp.WebAPI/Controllers/TokenController.cs` (lines 104, 146–148)
- **CWE**: [CWE-287](https://cwe.mitre.org/data/definitions/287.html) (Improper Authentication)
- **OWASP**: API2:2023 Broken Authentication
- **CVSS v3.1 Base Score**: **4.8** (`CVSS:3.1/AV:N/AC:L/PR:L/UI:N/S:U/C:N/I:L/A:N`)
- **Severity**: 🔵 **Low**
- **Description**: `TokenController` assigned an empty `TokenDTO` to `response.Object`. Additionally, `claims.ToList().AddRange(...)` operated on a discarded copy, dropping all role claims from issued tokens.
- **Attack Scenario**: Legitimate administrators logging in received tokens lacking role claims, leading to unexpected authorization failures or prompting insecure workarounds.
- **Remediation**: Assigned the generated token object to `response.Object` and properly appended role claims to the claims list before token signing.
- **Remediation Status**: ✅ **Fixed**
- **Verification Status**: ✅ Verified with unit test `GeneratedToken_MustContain_RoleClaims`.

---

### SEC-014: Production Debug Logging
- **Title**: Verbose Debug Logging Active in Production Configuration
- **Category**: Security Misconfiguration
- **Affected Component**: Logging Subsystem
- **Affected File**: `src/WebUI/appsettings.Production.json` (line 5)
- **CWE**: [CWE-532](https://cwe.mitre.org/data/definitions/532.html) (Insertion of Sensitive Information into Log File)
- **OWASP**: A05:2021 Security Misconfiguration
- **CVSS v3.1 Base Score**: **4.3** (`CVSS:3.1/AV:N/AC:L/PR:L/UI:N/S:U/C:L/I:N/A:N`)
- **Severity**: 🔵 **Low**
- **Description**: Production configuration specified `"Default": "Debug"` for log level, capturing query strings, internal states, and diagnostic payloads.
- **Remediation**: Set default production logging level to `"Warning"`.
- **Remediation Status**: ✅ **Fixed**
- **Verification Status**: ✅ Verified via configuration inspection.

---

### SEC-015: Synchronous-over-Async Thread Pool Exhaustion in Role Queries
- **Title**: Synchronous .Result Invocation inside LINQ Predicates
- **Category**: Resource Management / Denial of Service
- **Affected Component**: Account Controller Queries
- **Affected File**: `src/ArrayApp.WebAPI/Controllers/AccountController.cs` (lines 559, 569, 580)
- **CWE**: [CWE-400](https://cwe.mitre.org/data/definitions/400.html) (Uncontrolled Resource Consumption)
- **OWASP**: A05:2021 Security Misconfiguration
- **CVSS v3.1 Base Score**: **4.3** (`CVSS:3.1/AV:N/AC:L/PR:L/UI:N/S:U/C:N/I:N/A:L`)
- **Severity**: 🔵 **Low**
- **Description**: Calling `.Result` synchronously inside LINQ iterations caused thread blocking and thread pool starvation under moderate concurrency.
- **Remediation**: Replaced synchronous calls with native asynchronous `_userManager.GetUsersInRoleAsync(roleName)`.
- **Remediation Status**: ✅ **Fixed**
- **Verification Status**: ✅ Verified via code review and solution test suite.

---

### SEC-016: Missing External Secrets Provider in Production Deployments
- **Title**: Application Relies on JSON Configuration Files for Secret Storage
- **Category**: Secrets Management
- **Affected Component**: Infrastructure Deployment Configuration
- **Affected File**: `appsettings.json`, `appsettings.Production.json`
- **CWE**: [CWE-522](https://cwe.mitre.org/data/definitions/522.html) (Insufficiently Protected Credentials)
- **OWASP**: A05:2021 Security Misconfiguration
- **CVSS v3.1 Base Score**: **3.8** (`CVSS:3.1/AV:L/AC:L/PR:H/UI:N/S:U/C:L/I:L/A:N`)
- **Severity**: 🔵 **Low**
- **Description**: Secrets are structured inside `appsettings.json` templates rather than pulled at runtime from a dedicated secrets vault (Azure Key Vault, AWS Secrets Manager, HashiCorp Vault).
- **Remediation**: Code-level placeholder secured. Operational implementation of Azure Key Vault or environment secret injection required in target production cloud environment.
- **Remediation Status**: ⏳ **Operational Action Required** (Documented in Security Roadmap)
- **Verification Status**: Pending production deployment pipeline configuration.

---

### SEC-017: Outdated CI/CD Action Versions and Playwright Path Mismatch
- **Title**: Deprecated GitHub Actions and Outdated Net7.0 Playwright Scripts
- **Category**: Supply Chain / Pipeline Hygiene
- **Affected Component**: GitHub Workflows
- **Affected File**: `.github/workflows/live-tests.yml`, `package.yml`, `codeql-analysis.yml`
- **CWE**: [CWE-1104](https://cwe.mitre.org/data/definitions/1104.html)
- **OWASP**: A06:2021 Vulnerable and Outdated Components
- **CVSS v3.1 Base Score**: **2.5**
- **Severity**: ⚪ **Informational**
- **Description**: Workflows referenced v1 and v2 actions and targeted net7.0 paths in Playwright install.
- **Remediation**: Upgraded actions across all workflows to v4/v3.
- **Remediation Status**: ✅ **Fixed**
- **Verification Status**: ✅ Verified via YAML inspection.
