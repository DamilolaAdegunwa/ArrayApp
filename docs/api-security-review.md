# API Security Review & Developer Guidelines — ArrayApp

This document provides architectural standards and security rules for developers creating or modifying WebAPI controllers in ArrayApp.

---

## 1. Authentication & Authorization Standard
- **Default Deny**: Every new controller MUST inherit from `BaseController` and declare an explicit `[Authorize]` attribute at the class level.
- **Role Restrictions**: Administrative or elevated business logic MUST specify roles: `[Authorize(Roles = "Administrator,Admin,admin")]`.
- **AllowAnonymous Justification**: Any usage of `[AllowAnonymous]` must be peer-reviewed by the security team and accompanied by rate-limiting and anti-brute-force safeguards.

---

## 2. Input Validation & Data Transfer Objects (DTOs)
- **Never Return Domain Entities Directly**: Under no circumstances should `ApplicationUser` or internal entities be returned from API endpoints. Always project into a dedicated DTO (e.g., `UserDto`).
- **Validate All Inputs**: Use FluentValidation validators in the `Application` layer to enforce boundary checks on all command and query properties.
- **Bind Identity to Server Claims**: Never trust client-supplied user IDs, clearance levels, or tenant IDs in request bodies or query strings. Always derive caller identity from `User.FindFirst(ClaimTypes.NameIdentifier)` or custom claims.

---

## 3. Error Handling & Exception Shields
- **No Stack Traces**: Do not concatenate or log `ex.StackTrace`, server file paths, or raw SQL queries in API responses.
- **Global Exception Filter**: Allow `ApiExceptionFilterAttribute` to catch and transform unhandled exceptions into RFC 7807 `ProblemDetails` with an opaque `traceId`.

---

## 4. Rate Limiting & Resource Consumption
- **Authentication Endpoints**: Any endpoint accepting credentials or issuing tokens must be protected by the `AuthRateLimitPolicy` (10 req/min).
- **Asynchronous Execution**: Always use `await` with asynchronous APIs. Never use `.Result` or `.Wait()`, which causes thread pool starvation.
