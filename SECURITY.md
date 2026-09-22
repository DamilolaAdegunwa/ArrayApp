# Security Policy — ArrayApp

ArrayApp is committed to ensuring the security, integrity, and privacy of our software and users. This document outlines our security policies, supported versions, and procedures for reporting potential security vulnerabilities.

---

## 1. Supported Versions

We actively maintain and provide security patches for the following versions:

| Version | Target Framework | Support Level | Status |
| :--- | :--- | :--- | :--- |
| **`1.0.x`** | `.NET 10.0` | **Actively Supported** | Supported (Current Release) |
| `< 1.0.0` | `.NET 7.0` (EOL) | **End-of-Life** | Unsupported |

---

## 2. Reporting a Vulnerability

If you discover a security vulnerability within ArrayApp, please adhere to responsible disclosure principles and report the issue directly to our security team before publishing or discussing it publicly.

### How to Submit a Report:
- **Email**: Send detailed vulnerability reports to `security@arrayapp.internal` (or use GitHub Private Vulnerability Reporting on this repository).
- **PGP Encryption**: If sending sensitive proof-of-concept material via email, please encrypt using our PGP public key (fingerprint available upon request).

### What to Include in Your Report:
To help us triage and investigate the issue quickly, please provide:
1. **Description**: Clear description of the vulnerability and its potential impact.
2. **Affected Component**: Specific API endpoint, controller, model, or configuration file.
3. **Reproduction Steps**: Step-by-step instructions or non-destructive proof-of-concept requests.
4. **CWE / Attack Classification**: If known (e.g., BOLA, IDOR, SQLi, CSRF).
5. **Mitigation Suggestion**: Any proposed fix or mitigation.

### Our Response Commitment:
- **Initial Acknowledgment**: Within **24 business hours**.
- **Triage & Assessment**: Within **72 business hours**.
- **Fix Delivery & Disclosure**: Within **30 calendar days**, coordinated with the finder.

---

## 3. Security Architecture & Guarantees

ArrayApp incorporates modern enterprise application security controls:

- **Zero-Trust Default Deny**: All business controllers enforce cryptographic JWT Bearer authentication via `[Authorize]`.
- **Role-Based Access Control (RBAC)**: Privileged administrative endpoints mandate the `Administrator` role.
- **Data Protection by Default**: User entities are serialized via safe Data Transfer Objects (`UserDto`), preventing credential and security stamp leakage.
- **Cryptographic Rigor**: Symmetric JWT signing enforces keys of at least 256 bits (32 bytes), with issuer, audience, and lifetime validation.
- **Resource Protection & Rate Limiting**: Authentication endpoints enforce fixed-window rate limiting (10 req/min) and Identity account lockout (5 failed attempts).
- **Hardened Error Handling**: Information disclosure is blocked; unhandled exceptions return sanitized RFC 7807 `ProblemDetails` with unique `traceId` correlation codes.
- **Container Hardening**: Multi-stage Docker containers execute on modern .NET 10 LTS runtimes as an unprivileged service account (`appuser:appgroup`, UID 10001) with native health monitoring.

---

## 4. Security Audit Documentation

Comprehensive security documentation generated during our independent AppSec assessment is available in the [`security-audit/`](security-audit/) directory:
- [`security-audit/threat-model.md`](security-audit/threat-model.md) — STRIDE Threat Model & Attack Trees
- [`security-audit/risk-register.md`](security-audit/risk-register.md) — Detailed Risk Register
- [`security-audit/api-security-matrix.md`](security-audit/api-security-matrix.md) — API Endpoint Security Matrix
- [`security-audit/remediation-report.md`](security-audit/remediation-report.md) — Source Code Remediation Changelog
- [`security-audit/security-test-results.md`](security-audit/security-test-results.md) — Automated Security Test Results
- [`security-audit/security-roadmap.md`](security-audit/security-roadmap.md) — Strategic Security Roadmap
- [`security-audit/devsecops-pipeline.md`](security-audit/devsecops-pipeline.md) — DevSecOps Pipeline Architecture
