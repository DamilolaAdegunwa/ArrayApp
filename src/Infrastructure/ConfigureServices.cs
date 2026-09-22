#pragma warning disable
#pragma info disable
using System.Data;
using ArrayApp.Application.Common.Interfaces;
using ArrayApp.Domain.Entities;
using ArrayApp.Infrastructure.Files;
using ArrayApp.Infrastructure.Identity;
using ArrayApp.Infrastructure.Persistence;
using ArrayApp.Infrastructure.Persistence.Interceptors;
using ArrayApp.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<AuditableEntitySaveChangesInterceptor>();

        if (configuration.GetValue<bool>("UseInMemoryDatabase"))
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("ArrayAppDb"));
        }
        else
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                    builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
        }

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<ApplicationDbContextInitialiser>();

        //services
        //    .AddDefaultIdentity<ApplicationUser>()
        //    //.AddRoles<IdentityRole>()
        //    .AddRoles<ApplicationRole>()
        //    .AddEntityFrameworkStores<ApplicationDbContext>();

        //services.AddIdentityServer().AddApiAuthorization<ApplicationUser, ApplicationDbContext>();

        services.AddIdentity<ApplicationUser, ApplicationRole>(options => {
            // Hardened password options aligned with OWASP ASVS & NIST SP 800-63B
            options.Password.RequireDigit = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
            options.Password.RequiredLength = 8;
            options.Password.RequiredUniqueChars = 2;

            // Hardened lockout protection against brute-force and credential stuffing
            options.Lockout.AllowedForNewUsers = true;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            options.Lockout.MaxFailedAccessAttempts = 5;

            options.SignIn.RequireConfirmedEmail = false;
            options.SignIn.RequireConfirmedAccount = false;
            options.SignIn.RequireConfirmedPhoneNumber = false;
            options.User.RequireUniqueEmail = true;
        })
                .AddRoles<ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
        services.AddControllersWithViews();

        services.AddTransient<IDateTime, DateTimeService>();
        services.AddTransient<IIdentityService, IdentityService>();
        services.AddTransient<ICsvFileBuilder, CsvFileBuilder>();

        // Idea Cultivation Platform Services
        services.AddScoped<IAIAgentService, AIAgentService>();
        services.AddScoped<IConnectorService, ConnectorService>();
        services.AddScoped<IReputationService, ReputationService>();
        services.AddScoped<IIdeaProductService, IdeaProductService>();
        services.AddScoped<IRoleCapacityService, RoleCapacityService>();
        services.AddScoped<ISessionPlaybookService, SessionPlaybookService>();
        services.AddSignalR();

        services.AddAuthentication()
            .AddIdentityServerJwt();

        services.AddAuthorization(options =>
            options.AddPolicy("CanPurge", policy => policy.RequireRole("Administrator")));

        return services;
    }
}
