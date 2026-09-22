# Security Roadmap & Implementation Milestones — ArrayApp

This document outlines the four strategic security horizons established to maintain and advance ArrayApp's security posture over the next 12 months.

---

## Roadmap Horizons

### Horizon 1: Immediate Actions (Days 0 - 7)
- **Production Secret Injection**: Configure Azure Key Vault or AWS Secrets Manager to inject runtime JWT keys and database connection strings dynamically.
- **GitHub Secrets Configuration**: Verify that `SQL_SA_PASSWORD` is configured in GitHub repository secrets for continuous integration runs.
- **CI Security Gate**: Ensure `dotnet test tests/Security.UnitTests/` executes on every pull request.

### Horizon 2: Short-Term Enhancements (Weeks 1 - 4)
- **Cryptographic Refresh Tokens**: Add refresh tokens with single-use rotation and explicit revocation lists.
- **Multi-Factor Authentication (MFA)**: Enforce TOTP-based 2FA for users in the `Administrator` role.
- **Git History Scrubbing**: Execute `git-filter-repo` to purge historical development test keys prior to public release.
- **Automated SCA**: Enable GitHub Dependabot or Snyk for automated dependency scanning.

### Horizon 3: Medium-Term Hardening (Months 1 - 3)
- **Centralized SIEM Telemetry**: Forward structured security audit logs to Azure Sentinel, Splunk, or Datadog.
- **Container Vulnerability Scanning**: Integrate Aqua Trivy into GitHub Actions to scan Docker images on build.
- **Open Policy Agent (OPA)**: Externalize complex ABAC zero-trust policies to OPA / Rego rules.

### Horizon 4: Enterprise Maturity (Months 3 - 12)
- **SOC 2 Type II & ISO 27001 Certification**: Complete formal compliance audits.
- **Bug Bounty Program**: Launch a coordinated vulnerability disclosure and bug bounty program.
- **Service Mesh & mTLS**: Deploy Istio or Linkerd to enforce mutual TLS across internal service communications.
