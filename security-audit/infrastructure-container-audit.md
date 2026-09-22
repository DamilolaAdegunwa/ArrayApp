# Infrastructure & Container Security Audit — ArrayApp

**Assessment Date**: 2026-09-22  
**Auditor**: Independent Application Security & Cybersecurity Audit Team  
**Scope**: Dockerfile, `.dockerignore`, GitHub Actions workflows, container runtime security, infrastructure deployment  

---

## 1. Executive Summary

Container and infrastructure configurations were evaluated against CIS Docker Benchmarks, NIST SP 800-190 (Application Container Security Guide), and DevSecOps best practices.

Prior to remediation, the container definition presented critical security flaws:
1. Utilization of an End-of-Life base image (.NET 7).
2. Execution of application processes as `root` (UID 0).
3. Binding to privileged network ports (port 80).
4. Absence of container health checks.
5. Incomplete `.dockerignore` file allowing test binaries, secrets, and git metadata into container build contexts.
6. CI/CD pipelines utilizing hardcoded database passwords.

All of these vulnerabilities have been **fully remediated**.

---

## 2. Dockerfile Hardening & Review

### 2.1 Before vs After Comparison

#### Pre-Remediation Dockerfile:
```dockerfile
# INSECURE: EOL image, root user, privileged port, no healthcheck
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/ArrayApp.WebAPI/ArrayApp.WebAPI.csproj", "src/ArrayApp.WebAPI/"]
...
FROM build AS publish
RUN dotnet publish "ArrayApp.WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ArrayApp.WebAPI.dll"]
```

#### Remediated Dockerfile:
```dockerfile
# SECURED: .NET 10 LTS, non-root user (10001), unprivileged port 8080, healthcheck
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080 \
    DOTNET_EnableDiagnostics=0 \
    ASPNETCORE_FORWARDEDHEADERS_ENABLED=true

# Create dedicated unprivileged service account
RUN addgroup --system --gid 10001 appgroup && \
    adduser --system --uid 10001 --ingroup appgroup --shell /sbin/nologin appuser

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/ArrayApp.WebAPI/ArrayApp.WebAPI.csproj", "src/ArrayApp.WebAPI/"]
COPY ["src/Application/Application.csproj", "src/Application/"]
COPY ["src/Domain/Domain.csproj", "src/Domain/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]
RUN dotnet restore "src/ArrayApp.WebAPI/ArrayApp.WebAPI.csproj"
COPY . .
WORKDIR "/src/src/ArrayApp.WebAPI"
RUN dotnet build "ArrayApp.WebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ArrayApp.WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Run as non-root user
USER appuser:appgroup

# Container Healthcheck
HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "ArrayApp.WebAPI.dll"]
```

---

## 3. CIS Docker Benchmark Compliance Checklist

| CIS Control | Description | Status | Verification Detail |
| :--- | :--- | :--- | :--- |
| **CIS 4.1** | Create a user for the container | **PASS** | Container creates and switches to non-root `appuser` (UID 10001) |
| **CIS 4.2** | Use trusted base images | **PASS** | Official Microsoft .NET 10 base images from Microsoft Container Registry (`mcr.microsoft.com`) |
| **CIS 4.3** | Do not install unnecessary packages | **PASS** | Uses minimal runtime base image without compilers or debuggers |
| **CIS 4.6** | Add HEALTHCHECK instruction | **PASS** | Native `HEALTHCHECK` verifies service availability every 30s |
| **CIS 4.7** | Do not use update instructions alone | **PASS** | Multi-stage build prevents package manager caching |
| **CIS 4.9** | Use COPY instead of ADD | **PASS** | Strictly uses `COPY` across all stages |
| **CIS 4.10**| Do not store secrets in Dockerfiles | **PASS** | Zero credentials or tokens present in build files |
| **CIS 5.7** | Do not map privileged host ports | **PASS** | Port 8080 bound; port 80/443 avoided |

---

## 4. Build Context Hardening (`.dockerignore`)

The `.dockerignore` file was expanded and hardened to prevent sensitive files, build artifacts, and developer state from leaking into Docker images during build context transfer:

```
**/.git
**/.gitignore
**/.vs
**/.vscode
**/.idea
**/bin
**/obj
**/TestResults
**/*.user
**/*.suo
**/*.pfx
**/*.key
**/*.pem
**/.env
**/.env.*
**/node_modules
**/.DS_Store
tests/
security-audit/
docs/
```

---

## 5. CI/CD Pipeline Security Hardening

### Workflow Analysis:
1. **`.github/workflows/dotnet-build.yml`**:
   - Upgraded to `actions/checkout@v4` and `actions/upload-artifact@v4`.
   - SA password extracted to `${{ secrets.SQL_SA_PASSWORD }}`.
2. **`.github/workflows/dotnet-deploy.yml`**:
   - Replaced deprecated `::set-output` syntax with `$GITHUB_OUTPUT`.
   - Upgraded container registry deployment actions.
3. **`.github/workflows/codeql-analysis.yml`**:
   - Upgraded CodeQL analysis engine to `github/codeql-action@v3`.
   - Configured security-extended queries for comprehensive C# CWE coverage.

---

## 6. Recommended Container Runtime Hardening (Kubernetes / Docker Compose)

When deploying ArrayApp in production Kubernetes or Docker environments, enforce the following security context:

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: arrayapp-webapi
spec:
  template:
    spec:
      securityContext:
        runAsNonRoot: true
        runAsUser: 10001
        runAsGroup: 10001
        fsGroup: 10001
      containers:
      - name: webapi
        image: arrayapp-webapi:latest
        securityContext:
          allowPrivilegeEscalation: false
          readOnlyRootFilesystem: true
          capabilities:
            drop:
              - ALL
        resources:
          limits:
            cpu: "1000m"
            memory: "1Gi"
          requests:
            cpu: "250m"
            memory: "256Mi"
```
