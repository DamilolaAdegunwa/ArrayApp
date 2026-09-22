# Security Findings & Vulnerability Summary — ArrayApp

This document summarizes the 17 security findings identified during the security audit and their corresponding remediation status.

---

## Findings Matrix

| Finding ID | Title | Severity | CWE | OWASP API (2023) | Status |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **SEC-001** | BOLA Password Reset Account Takeover | **CRITICAL** | CWE-639 | API1: Broken Object Level Authorization | **REMEDIATED** |
| **SEC-002** | Vertical Privilege Escalation in Role Management | **CRITICAL** | CWE-285 | API5: Broken Function Level Authorization | **REMEDIATED** |
| **SEC-003** | Missing Authentication on 35 API Controllers | **CRITICAL** | CWE-306 | API2: Broken Authentication | **REMEDIATED** |
| **SEC-004** | Sensitive Data Exposure (Password Hash & Stamps) | **HIGH** | CWE-200 | API3: Broken Object Property Level Auth | **REMEDIATED** |
| **SEC-005** | Internal Exception Stack Trace Disclosure | **HIGH** | CWE-209 | API8: Security Misconfiguration | **REMEDIATED** |
| **SEC-006** | Inadequate JWT Key Length (128-bit) & Validation | **HIGH** | CWE-326 | API2: Broken Authentication | **REMEDIATED** |
| **SEC-007** | Missing Rate Limiting & Account Lockout | **HIGH** | CWE-307 | API4: Unrestricted Resource Consumption | **REMEDIATED** |
| **SEC-008** | Client-Controlled ABAC Attributes in Zero-Trust | **HIGH** | CWE-639 | API1: Broken Object Level Authorization | **REMEDIATED** |
| **SEC-009** | Container Security: EOL Base Image & Root User | **MEDIUM** | CWE-250 | API8: Security Misconfiguration | **REMEDIATED** |
| **SEC-010** | Hardcoded CI/CD Passwords & Deprecated Actions | **MEDIUM** | CWE-798 | API8: Security Misconfiguration | **REMEDIATED** |
| **SEC-011** | Permissive Password Complexity Requirements | **MEDIUM** | CWE-521 | API2: Broken Authentication | **REMEDIATED** |
| **SEC-012** | Missing Defensive Security Response Headers | **MEDIUM** | CWE-693 | API8: Security Misconfiguration | **REMEDIATED** |
| **SEC-013** | Excessive JWT Token Validity Duration (50h) | **LOW** | CWE-613 | API2: Broken Authentication | **REMEDIATED** |
| **SEC-014** | Sync-Over-Async Thread Pool Starvation Hazard | **LOW** | CWE-400 | API4: Unrestricted Resource Consumption | **REMEDIATED** |
| **SEC-015** | Unencrypted Database Connection String Defaults | **LOW** | CWE-319 | API8: Security Misconfiguration | **REMEDIATED** |
| **SEC-016** | Production Logging Defaults to Debug | **INFO** | CWE-532 | API9: Improper Inventory Management | **REMEDIATED** |
| **SEC-017** | Historic Development Secrets in Git Commit Log | **INFO** | CWE-312 | API8: Security Misconfiguration | **OPERATIONAL** |

For the complete technical breakdown, CVSS scores, and code diffs, refer to [`security-audit/risk-register.md`](../security-audit/risk-register.md) and [`security-audit/remediation-report.md`](../security-audit/remediation-report.md).
