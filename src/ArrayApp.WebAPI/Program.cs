using System;
using System.Text;
using ArrayApp.Application.Common.Models;
using ArrayApp.Infrastructure.Persistence;
using ArrayApp.Infrastructure.Repositories;
using ArrayApp.Infrastructure.Repositories.Interfaces;
using ArrayApp.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using AutoMapper;
using ArrayApp.Application.Common.Mappings;
using Microsoft.AspNetCore.Hosting;
using MABSwagger = Microsoft.AspNetCore.Builder.SwaggerBuilderExtensions;
using System.Reflection.Metadata;
using ArrayApp.Application.Common.Interfaces;
using ArrayApp.Infrastructure.Services.Interfaces;
using Steeltoe.Extensions.Configuration.ConfigServer;
namespace ArrayApp.WebAPI;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args)
            .AddConfigServer()
            ;

        string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        string name = builder.Configuration["name"];
        string age = builder.Configuration["age"];
        string country = builder.Configuration["country"];
        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
        builder.Services.AddApplicationServices();// from the application's dll
        builder.Services.AddInfrastructureServices(builder.Configuration);// from the infrastructure dll
        builder.Services.Configure<JwtConfig>(options => builder.Configuration.GetSection(Constants.Sections.AuthJwtBearer).Bind(options));
        builder.Services.AddScoped<IAccountService, AccountService>();
        builder.Services.AddScoped<IServiceHelper, ServiceHelper>();
        builder.Services.AddScoped<ITokenSvc, TokenService>();
        builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
        builder.Services.AddWebAPIServices();// web api services
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

        #region jwt
        var jwtKey = builder.Configuration.GetSection(Constants.Sections.AuthJwtBearer).GetValue<string>("SecurityKey");

        builder.Services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(x =>
        {
            x.Authority = "https://YOUR_AUTH0_DOMAIN";
            x.Audience = "YOUR_AUDIENCE";
            //----
            x.RequireHttpsMetadata = false;
            x.SaveToken = true;
            x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtKey)),
                ValidateIssuer = false,
                ValidateAudience = false,
                //---
                ValidAudiences = new string[] { "YOUR_AUDIENCE" },
                ValidIssuer = "YOUR_AUTH0_DOMAIN"
            };
        });
        //simplified version
        //builder.Services.AddAuthentication().AddJwtBearer(); //?? new feature
        #endregion

        var app = builder.Build();

        #region old impl of swagger
        //// Configure the HTTP request pipeline.
        //if (app.Environment.IsDevelopment())
        //{
        //    //app.UseSwagger();
        //    app.UseSwaggerUI();
        //}
        #endregion

        #region newer
        // Configure the HTTP request pipeline.
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
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHealthChecks("/health");
        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseOpenApi();
        //app.UseSwaggerUi3();

        app.UseSwaggerUi3(settings =>
        {
            settings.Path = "/swagger";
            //settings.Path = "/api";
            //settings.DocumentPath = "/api/specification.json";
        });

        app.UseRouting();

        app.UseAuthentication();
        #endregion

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
/*
 -- Insert roles into AspNetRoles table
INSERT INTO AspNetRoles (Name)
VALUES
    ('admin'),
    ('manager'),
    ('finance'),
    ('accountant'),
    ('business'),
    ('marketing'),
    ('hr'),
    ('security'),
    ('operations'),
    ('support'),
    ('customercare'),
    ('ceo'),
    ('audit');

-- Insert permissions into Permissions table
INSERT INTO Permissions (Name)
VALUES
    ('dashboard_edit_and_view'),
    ('dashboard_view'),
    ('payment_edit_and_view'),
    ('payment_view'),
    ('subscription_edit_and_view'),
    ('subscription_view'),
    ('campaign_edit_and_view'),
    ('campaign_view'),
    ('moderator_edit_and_view'),
    ('moderator_view');
 */