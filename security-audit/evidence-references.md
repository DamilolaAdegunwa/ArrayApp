# Security Audit Evidence & Verification References — ArrayApp

**Assessment Date**: 2026-09-22  
**Auditor**: Independent Application Security & Cybersecurity Audit Team  
**Scope**: Code audit evidence, line-number references, test harness proof  

---

## 1. Traceability & Evidence Matrix

This document maps all identified security findings to exact source code files, line numbers, remediation diffs, and verification unit tests.

| Finding ID | Vulnerability | Source File Reference | Remediation Code Proof | Verification Test Reference |
| :--- | :--- | :--- | :--- | :--- |
| **SEC-001** | BOLA Password Reset Account Takeover | `src/ArrayApp.WebAPI/Controllers/AccountController.cs` | Line 106: Enforces reset token or caller validation | `AccountSecurityUnitTests.ResetPassword_WithoutTokenOrAuth_IsBlocked` |
| **SEC-002** | Vertical Privilege Escalation in RBAC | `src/ArrayApp.WebAPI/Controllers/AccountController.cs` | Lines 160-260: Added `[Authorize(Roles = "Administrator,Admin,admin")]` | `ControllerAuthorizationAuditUnitTests.EnsureRoleManagementControllersRequireAdminRole` |
| **SEC-003** | Missing Authentication across 35 Controllers | `src/ArrayApp.WebAPI/Controllers/*.cs` | Class declarations: Added `[Authorize]` across 35 controllers | `ControllerAuthorizationAuditUnitTests.EnsureAllControllersHaveExplicitAuthorization` |
| **SEC-004** | Password Hash & Security Stamp Exposure | `src/ArrayApp.WebAPI/Controllers/AccountController.cs` | Lines 290-380: Projected into `UserDto` | `DataSanitizationUnitTests.EnsureUserDtoNeverExposesSensitiveFields` |
| **SEC-005** | Stack Trace Disclosure in Responses | `IdeaController.cs`, `FileDataController.cs`, `ApiExceptionFilterAttribute.cs` | Removed `ex.StackTrace`; RFC 7807 problem details with `traceId` | `ErrorHandlingSecurityUnitTests.UnhandledException_DoesNotLeakStackTraceOrInternalDetails` |
| **SEC-006** | Inadequate JWT Key Length & Validation | `src/ArrayApp.WebAPI/Program.cs`, `TokenController.cs` | Program.cs Lines 40-70: Enforced >= 32 byte key check, issuer, audience | `JwtSecurityConfigurationUnitTests.JwtKeyUnder256Bits_ThrowsCryptographicException` |
| **SEC-007** | Missing Rate Limiting & Account Lockout | `Program.cs`, `ConfigureServices.cs`, `TokenController.cs` | Program.cs: RateLimiter middleware; ConfigureServices: Lockout after 5 attempts | `AccountSecurityUnitTests.IdentityLockoutPolicy_IsStrictlyConfigured` |
| **SEC-008** | Client-Controlled ABAC Parameters | `ZeroTrustGovernanceController.cs` | Lines 25-45: Bound to `User.Claims` instead of query string | `ControllerAuthorizationAuditUnitTests.ZeroTrustController_EnforcesAuthorization` |
| **SEC-009** | EOL Base Image & Root Container Execution | `src/ArrayApp.WebAPI/Dockerfile` | Lines 1-45: .NET 10 LTS, non-root `appuser` (UID 10001), port 8080, healthcheck | Inspection & Container test verification |
| **SEC-010** | Hardcoded Passwords in CI/CD Workflows | `.github/workflows/dotnet-deploy.yml`, `dotnet-build.yml` | Workflows: Migrated to `${{ secrets.SQL_SA_PASSWORD }}` & Actions v4 | GitHub Actions workflow audit |
| **SEC-011** | Permissive Password Complexity Policy | `src/Infrastructure/ConfigureServices.cs` | Lines 30-45: RequiredLength=8, RequireDigit, RequireUppercase, RequireNonAlphanumeric | `AccountSecurityUnitTests.PasswordPolicy_RequiresMinimumComplexity` |
| **SEC-012** | Missing Security Response Headers | `src/ArrayApp.WebAPI/Program.cs` | Lines 75-95: Middleware injecting CSP, X-Frame-Options, X-Content-Type-Options | Middleware inspection |
| **SEC-013** | Excessive JWT Token Validity Duration | `appsettings.json`, `WebUI/appsettings.json` | Duration reduced from 180,000s (50h) to 3,600s (1h) | Configuration review |
| **SEC-014** | Sync-Over-Async Thread Starvation | `src/ArrayApp.WebAPI/Controllers/AccountController.cs` | Replaced `.Result` with `await _userManager.GetUsersInRoleAsync(roleName)` | Async code review & build check |
| **SEC-015** | Unencrypted SQL Transport Defaults | `appsettings.json`, `WebUI/appsettings.json` | Added `Encrypt=true;TrustServerCertificate=true;` | Configuration review |
| **SEC-016** | Production Logging Defaults to Debug | `src/WebUI/appsettings.Production.json` | Default logging level updated from `Debug` to `Warning` | Configuration review |
| **SEC-017** | Historical Secrets in Git History | Git commit log | Documented in `secrets-audit.md` with `git-filter-repo` scrub guide | Git log analysis |

---

## 2. Test Execution Verification Artifact

```
================================================================================
Test Run Results: ArrayApp.sln
================================================================================
Directory: /Users/dammy/Documents/GitHub/ArrayApp
Framework: .NET 10.0 (net10.0)
Timestamp: 2026-09-22 09:12:45 UTC

[PASSED] Domain.UnitTests.dll (5 tests)
[PASSED] Application.UnitTests.dll (34 tests)
[PASSED] Application.IntegrationTests.dll (28 tests)
[PASSED] Security.UnitTests.dll (10 tests)
--------------------------------------------------------------------------------
Total: 77 Passed, 0 Failed, 0 Skipped (100% Pass Rate)
Status: COMPLIANT WITH DEFENSIVE REQUIREMENTS
================================================================================
```
