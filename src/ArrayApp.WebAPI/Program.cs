using System;
using System.Text;
using System.Threading.RateLimiting;
using ArrayApp.Application.Common.Interfaces;
using ArrayApp.Application.Common.Mappings;
using ArrayApp.Application.Common.Models;
using ArrayApp.Infrastructure.Persistence;
using ArrayApp.Infrastructure.Repositories;
using ArrayApp.Infrastructure.Repositories.Interfaces;
using ArrayApp.Infrastructure.Services;
using ArrayApp.Infrastructure.Services.Interfaces;
using ArrayApp.WebAPI.Filters;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Steeltoe.Extensions.Configuration.ConfigServer;

namespace ArrayApp.WebAPI;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        string connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? string.Empty;

        // Add services to the container
        builder.Services.AddControllers(options =>
        {
            options.Filters.Add<ApiExceptionFilterAttribute>();
        });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddApplicationServices();
        builder.Services.AddInfrastructureServices(builder.Configuration);

        builder.Services.Configure<JwtConfig>(options => 
            builder.Configuration.GetSection(Constants.Sections.AuthJwtBearer).Bind(options));

        builder.Services.AddScoped<IAccountService, AccountService>();
        builder.Services.AddScoped<IServiceHelper, ServiceHelper>();
        builder.Services.AddScoped<ITokenSvc, TokenService>();
        builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
        builder.Services.AddWebAPIServices();

        builder.Services.AddScoped<IAdvertService, AdvertService>();
        builder.Services.AddScoped<IAppService, AppService>();
        builder.Services.AddScoped<ICategoryService, CategoryService>();
        builder.Services.AddScoped<IChatService, ChatService>();
        builder.Services.AddScoped<ICommentService, CommentService>();
        builder.Services.AddScoped<IFileDataService, FileDataService>();
        builder.Services.AddScoped<IUserGroupService, UserGroupService>();
        builder.Services.AddScoped<IIdeaService, IdeaService>();
        builder.Services.AddScoped<INotificationService, NotificationService>();
        builder.Services.AddScoped<ISessionService, SessionService>();
        builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
        builder.Services.AddScoped<ITagService, TagService>();
        builder.Services.AddScoped<IProductService, ProductService>();
        builder.Services.AddScoped<IUserRoleService, UserRoleService>();

        // Restrictive CORS Policy
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("ArrayAppCorsPolicy", policy =>
            {
                policy.WithOrigins(
                    "https://localhost:44447", 
                    "https://localhost:5001", 
                    "https://localhost:7089", 
                    "http://localhost:4200")
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
            });
        });

        // Rate Limiting Protection (Anti-Brute Force / Anti-DoS)
        builder.Services.AddRateLimiter(options =>
        {
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.User.Identity?.Name 
                                  ?? httpContext.Connection.RemoteIpAddress?.ToString() 
                                  ?? "anonymous",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = true,
                        PermitLimit = 120,
                        QueueLimit = 15,
                        Window = TimeSpan.FromMinutes(1)
                    }));
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        });

        #region Hardened JWT Bearer Authentication
        var jwtKey = builder.Configuration.GetSection(Constants.Sections.AuthJwtBearer).GetValue<string>("SecurityKey")
                     ?? "ArrayAppEnterpriseSecretKeyWith256BitsMinimumLength2026!";
        var jwtIssuer = builder.Configuration.GetSection(Constants.Sections.AuthJwtBearer).GetValue<string>("Issuer")
                        ?? "ArrayApp.WebUI.Issuer";
        var jwtAudience = builder.Configuration.GetSection(Constants.Sections.AuthJwtBearer).GetValue<string>("Audience")
                          ?? "ArrayApp.WebUI.Audience";

        var keyBytes = Encoding.UTF8.GetBytes(jwtKey);
        if (keyBytes.Length < 32)
        {
            var padded = new byte[32];
            Array.Copy(keyBytes, padded, keyBytes.Length);
            keyBytes = padded;
        }

        builder.Services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(x =>
        {
            x.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
            x.SaveToken = true;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                ValidateIssuer = true,
                ValidIssuer = jwtIssuer,
                ValidateAudience = true,
                ValidAudience = jwtAudience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(5)
            };
        });
        #endregion

        var app = builder.Build();

        // Configure the HTTP request pipeline
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseMigrationsEndPoint();

            // Initialise and seed database
            using (var scope = app.Services.CreateScope())
            {
                var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
                await initialiser.InitialiseAsync();
                await initialiser.SeedAsync();
            }
        }
        else
        {
            app.UseHsts();
        }

        // Security Response Headers Middleware
        app.Use(async (context, next) =>
        {
            context.Response.Headers["X-Content-Type-Options"] = "nosniff";
            context.Response.Headers["X-Frame-Options"] = "DENY";
            context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
            context.Response.Headers["Content-Security-Policy"] = 
                "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline'; img-src 'self' data:; connect-src 'self' wss: https:;";
            context.Response.Headers["Permissions-Policy"] = "geolocation=(), microphone=(), camera=()";
            await next();
        });

        app.UseHealthChecks("/health");
        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseCors("ArrayAppCorsPolicy");
        app.UseRateLimiter();

        app.UseOpenApi();
        app.UseSwaggerUi(settings =>
        {
            settings.Path = "/swagger";
        });

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.MapHub<ArrayApp.Infrastructure.Hubs.IdeaSessionHub>("/hubs/ideasession");
        app.MapHub<ArrayApp.Infrastructure.Hubs.IdeaChatHub>("/hubs/ideachat");

        app.Run();
    }
}