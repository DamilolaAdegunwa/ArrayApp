# ArrayApp Platform — Formal Security Threat Model (STRIDE)

**Document Version**: 2.0 (Pre-Production Pre-Flight)  
**Security Classification**: CONFIDENTIAL — INTERNAL & AUDIT USE ONLY  
**Assessment Target**: ArrayApp Enterprise Idea Cultivation & Work Orchestration Platform  
**Target Architecture**: Clean Architecture (.NET 10 / ASP.NET Core WebAPI & WebUI / EF Core / SQL Server / SignalR)  

---

## 1. Executive Summary & Scope

This formal Threat Model provides a structural security analysis of the **ArrayApp (IdeaApp)** ecosystem. Built upon the **STRIDE** methodology (Spoofing, Tampering, Repudiation, Information Disclosure, Denial of Service, Elevation of Privilege) developed by Microsoft, and aligned with the **OWASP Threat Dragon** and **NIST SP 800-154** (Guide to Data-Centric System Threat Modeling), this analysis identifies primary system assets, maps trust boundaries, catalogs potential threat actors, and decomposes attack vectors across the API, message hubs, and backend services.

---

## 2. Asset Inventory & Data Classification

| Asset Category | Specific Asset | Sensitivity Level | Business & Operational Impact if Compromised |
| :--- | :--- | :--- | :--- |
| **Authentication & Tokens** | JWT Signing Keys (HMAC-SHA256) | **Critical** | Complete token forgery, arbitrary authentication bypass across the entire platform. |
| **Credentials & Hashes** | User Password Hashes, Security Stamps | **Critical** | Offline cracking, persistent account impersonation, automated credential reuse. |
| **Database Credentials** | SQL Connection Strings, SA Passwords | **Critical** | Direct persistent storage compromise, unauthorized schema manipulation, data exfiltration. |
| **Confidential Business Data**| Unreleased Ideas, Patents, SWOT Analyses | **High** | Intellectual property theft, competitive sabotage, insider trading risks. |
| **Zero Trust ABAC Metadata**| Clearance Levels, Departmental Rules | **High** | Circumvention of institutional segregation of duties, unauthorized document clearance. |
| **Audit & Provenance** | Immutable Provenance Chain, Signatures | **High** | Falsification of authorship, legal repudiation of work contributions, audit trail destruction. |
| **Integrations & Webhooks** | Jira, GitHub, Slack, Linear Webhook Keys| **Medium-High** | Unauthorized command execution, malicious issue forging in enterprise trackers. |
| **Operational Telemetry** | Server Logs, Correlation Trace Identifiers | **Medium** | Architectural reconnaissance, internal network path mapping, exception probing. |

---

## 3. Trust Boundaries & Data Flow Decomposition

```
                         [ UNTRUSTED INTERNET ]
                                   │
                                   ▼ [HTTPS / Port 443 / 8080]
     ┌─────────────────────────────────────────────────────────────┐
     │ Trust Boundary 1: External Client ➔ WebAPI Gateway / WebUI │
     └─────────────────────────────┬───────────────────────────────┘
                                   │
               ┌───────────────────┴───────────────────┐
               ▼                                       ▼
    ┌──────────────────────┐               ┌──────────────────────┐
    │ REST API Controllers │               │ SignalR Realtime Hub │
    │   (JWT Validation)   │               │   (State Sync Mesh)  │
    └──────────┬───────────┘               └──────────┬───────────┘
               │                                       │
     ┌─────────┴───────────────────────────────────────┴───────────┐
     │ Trust Boundary 2: Web API Layer ➔ MediatR CQRS Pipeline     │
     └─────────────────────────────┬───────────────────────────────┘
                                   │
    ┌──────────────────────────────┴──────────────────────────────┐
    │ Application Layer Behaviors                                  │
    │  - AuthorizationBehavior (Policy / RBAC Enforcement)        │
    │  - ValidationBehavior (FluentValidation Rules)              │
    │  - PerformanceBehavior / UnhandledExceptionBehavior         │
    └──────────────────────────────┬──────────────────────────────┘
                                   │
     ┌─────────────────────────────┴───────────────────────────────┐
     │ Trust Boundary 3: Domain Core ➔ Infrastructure & Repositories│
     └─────────────────────────────┬───────────────────────────────┘
                                   │
         ┌─────────────────────────┼─────────────────────────┐
         ▼                         ▼                         ▼
┌─────────────────┐       ┌─────────────────┐       ┌─────────────────┐
│ ApplicationDbContext    │ External Service│       │ Zero Trust ABAC │
│ (EF Core SQL)   │       │ Connectors      │       │ Evaluator       │
│ - User Store    │       │ - GitHub / Jira │       │ - Clearance     │
│ - Idea Tables   │       │ - Webhook Rails │       │ - Blind Review  │
└─────────────────┘       └─────────────────┘       └─────────────────┘
         │                         │                         │
     ┌───┴─────────────────────────┴─────────────────────────┴─────┐
     │ Trust Boundary 4: Application ➔ Storage, Cloud & Integrations│
     └─────────────────────────────────────────────────────────────┘
```

### Trust Boundary Details
1. **Trust Boundary 1: Browser / Public Internet ➔ API & WebUI**:
   - **Protocol**: HTTP/HTTPS, WebSockets (WSS).
   - **Security Controls**: TLS 1.3 encryption, Rate Limiting Middleware, Security Response Headers (CSP, HSTS, X-Frame-Options), Host header validation.
2. **Trust Boundary 2: API Gateway ➔ MediatR CQRS Pipeline**:
   - **Mechanism**: In-memory message dispatching.
   - **Security Controls**: ClaimsPrincipal population, `[Authorize]` attributes, `AuthorizationBehaviour<TRequest, TResponse>`, `ValidationBehaviour<TRequest, TResponse>`.
3. **Trust Boundary 3: Application Layer ➔ Entity Framework Infrastructure**:
   - **Mechanism**: LINQ to Entities / DbContext.
   - **Security Controls**: Parameterized EF Core queries, `AuditableEntitySaveChangesInterceptor`, Tenancy filtering.
4. **Trust Boundary 4: Application ➔ External SaaS & CI/CD Connectors**:
   - **Protocol**: Outbound HTTPS Webhooks / REST APIs.
   - **Security Controls**: Webhook payload signing, outbound endpoint domain allowlisting, non-root container isolation.

---

## 4. Threat Actor Profiles

| Threat Actor | Motivation | Capabilities | Target Surfaces |
| :--- | :--- | :--- | :--- |
| **Unauthenticated Internet Attacker** | Opportunistic reconnaissance, credential stuffing, extortion | Automated vulnerability scanners, brute-force engines, public exploits | Exposed endpoints (`/api/*`), Swagger/OpenAPI docs, unauthenticated file upload, login portals |
| **Authenticated Low-Privilege User** | Horizontal data theft, privilege escalation, unauthorized access | Valid JWT bearer token, knowledge of internal object IDs | BOLA / IDOR endpoints (`ResetPassword`, `IdeaController`), role manipulation APIs, ABAC query bypass |
| **Malicious Insider / Employee** | Commercial espionage, intellectual property theft, sabotage | Authenticated corporate account, legitimate business access | Zero Trust clearance evaluations, blind-review reveal endpoints, audit log scraping |
| **Compromised Supply Chain** | Silent backdooring, credential harvesting, lateral movement | Injected dependencies in NuGet packages, npm modules, or GitHub Actions | CI/CD build runners, transitive package dependencies, base container images |
| **Infrastructure / Cloud Attacker** | Host compromise, container escape, lateral pivoting | Compromised container environment, cluster metadata access | Privileged container runtimes, exposed Docker socket, plaintext config secrets |

---

## 5. STRIDE Threat Analysis Matrix

| Threat Category | Threat Description | Vulnerable Component | Pre-Audit Vulnerability | Post-Remediation Control | Residual Risk |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **S** - Spoofing | Identity Spoofing via forged JWT tokens | `TokenController`, `Program.cs` | Disabled `ValidateIssuer` and `ValidateAudience`; weak 128-bit secret `C421AAEE0D114E9C`. | Strict 256-bit key validation, active issuer/audience validation, clock skew bounded to 5m. | Low |
| **S** - Spoofing | Clearance Spoofing in ABAC Evaluation | `ZeroTrustGovernanceController` | Department & Clearance accepted blindly from URL query strings. | ABAC attributes derived strictly from verified JWT user claims. | Low |
| **T** - Tampering | Account Takeover via BOLA / IDOR | `AccountController.ResetPassword` | Arbitrary password reset without token verification or current password. | Enforced caller ownership, required valid OTP reset token or current password; restricted admin overrides. | Low |
| **T** - Tampering | Provenance Chain Falsification | `ProvenanceController`, `ConnectorService` | Provenance logs modifiable or callable unauthenticated. | Class-level `[Authorize]` enforced; provenance logs append-only via EF Core interceptor. | Low |
| **R** - Repudiation | Unattributed Role Elevation | `AccountController.AddRolesToUser` | Role changes permitted to non-administrators without audit logs. | `[Authorize(Roles = "Administrator")]` enforced, structured security logging on all role mutations. | Low |
| **I** - Information Disclosure | PII & Credential Hash Leakage | `AccountController.GetAllUsers` | Raw `ApplicationUser` EF entities returned with `PasswordHash` & `SecurityStamp`. | Sanitized `UserDto` projection omitting confidential security properties. | Low |
| **I** - Information Disclosure | Stack Trace & Path Disclosures | `IdeaController`, `ApiExceptionFilterAttribute` | `ex.StackTrace` returned in HTTP 400 responses; unhandled 500s dumped to client. | Standardized RFC 7807 ProblemDetails returned with opaque correlation `traceId`; stack traces suppressed. | Low |
| **D** - Denial of Service | Thread Starvation / Deadlocks | `AccountController` role queries | Synchronous `.Result` invoked inside LINQ statements over async UserManager calls. | Fully asynchronous `GetUsersInRoleAsync` implementation. | Low |
| **D** - Denial of Service | Credential Stuffing & Endpoint Flooding | WebAPI Controllers, `/Token` | No rate limiting configured; infinite login attempts allowed. | ASP.NET Core Partitioned Fixed-Window Rate Limiter (120 req/min/IP). | Low |
| **E** - Elevation of Privilege | Vertical Privilege Escalation | `AccountController` Role Endpoints | Any authenticated user could invoke `AddRolesToUser` or `ResetRoles` to grant themselves `Administrator`. | Strict `[Authorize(Roles = "Administrator,Admin,admin")]` attribute applied to all role manipulation APIs. | Low |

---

## 6. Attack Trees

### Attack Tree 1: Account Takeover via BOLA Password Reset (Pre-Audit)
```
[Goal: Take Over Administrator Account]
  ├── 1. Discover target administrator user ID (e.g. ID "1" via /api/Account/GetAllUsers)
  ├── 2. Authenticate as low-privilege attacker via /SignUp and /Token
  └── 3. Exploit BOLA on /api/Account/ResetPassword
        ├── Pass {"userId": "1", "newPassword": "AttackerP@ss123"}
        └── Server invokes _userManager.GeneratePasswordResetTokenAsync(user) automatically
              └── Password reset succeeds without victim awareness
```
*Remediation Impact*: Mitigated. Cross-user password resets are blocked with HTTP 403 Forbidden; resets require verified `ResetToken` from user email/SMS OTP.

### Attack Tree 2: Unauthorized Role Elevation (Pre-Audit)
```
[Goal: Obtain Administrator Privileges]
  ├── 1. Register unprivileged user account
  ├── 2. Inspect API specification (/swagger or /api)
  └── 3. Call POST /api/Account/AddRolesToUser
        ├── Payload: {"userId": "my-id", "roleNames": ["Administrator"]}
        └── Controller lacks role authorization check
              └── User elevated to Administrator immediately
```
*Remediation Impact*: Mitigated. `AddRolesToUser`, `ResetRoles`, and `CreateRole` strictly enforce `[Authorize(Roles = "Administrator,Admin,admin")]`.

---

## 7. Threat Model Sign-Off
- **Lead Security Architect**: Antigravity Cybersecurity Audit Team  
- **Audit Date**: September 2026  
- **Review Status**: Approved & Verified with Automated Security Tests  
