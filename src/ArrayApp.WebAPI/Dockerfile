# Production Hardened Multi-Stage Dockerfile for ArrayApp.WebAPI
# Adheres to CIS Docker Benchmark & OWASP Container Security Guidelines

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

ENV ASPNETCORE_HTTP_PORTS=8080 \
    DOTNET_EnableDiagnostics=0 \
    DOTNET_CLI_TELEMETRY_OPTOUT=1

# Create unprivileged system user for non-root execution
RUN groupadd -g 10001 appgroup && \
    useradd -u 10001 -g appgroup -s /sbin/nologin -d /app appuser && \
    chown -R appuser:appgroup /app

# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project definition files for layer caching
COPY ["Directory.Build.props", "./"]
COPY ["src/Domain/ArrayApp.Domain.csproj", "src/Domain/"]
COPY ["src/Application/ArrayApp.Application.csproj", "src/Application/"]
COPY ["src/Infrastructure/ArrayApp.Infrastructure.csproj", "src/Infrastructure/"]
COPY ["src/ArrayApp.WebAPI/ArrayApp.WebAPI.csproj", "src/ArrayApp.WebAPI/"]

RUN dotnet restore "src/ArrayApp.WebAPI/ArrayApp.WebAPI.csproj"

# Copy source code and compile
COPY ["src/", "src/"]
WORKDIR "/src/src/ArrayApp.WebAPI"
RUN dotnet build "ArrayApp.WebAPI.csproj" -c Release -o /app/build --no-restore

# Publish Stage
FROM build AS publish
RUN dotnet publish "ArrayApp.WebAPI.csproj" -c Release -o /app/publish --no-restore /p:UseAppHost=false

# Final Hardened Runtime Container
FROM base AS final
WORKDIR /app
COPY --from=publish --chown=appuser:appgroup /app/publish .

# Enforce non-root execution
USER appuser:appgroup

# Container Healthcheck
HEALTHCHECK --interval=30s --timeout=5s --start-period=15s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "ArrayApp.WebAPI.dll"]