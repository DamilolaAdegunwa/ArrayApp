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
using Microsoft.FeatureManagement;
var builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");  //Configuration.GetConnectionString("DefaultConnection");

//builder.Services.AddDbContext(connectionString);
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
//builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
//.AddEntityFrameworkStores<ApplicationDbContext>()
//.AddDefaultTokenProviders();

// Add services to the container.
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddWebUIServices();
builder.Services.Configure<JwtConfig>(options => builder.Configuration.GetSection(Constants.Sections.AuthJwtBearer).Bind(options));
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IServiceHelper, ServiceHelper>();
builder.Services.AddScoped<ITokenSvc, TokenService>();
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();

#region jwt
var jwtKey = builder.Configuration.GetSection(Constants.Sections.AuthJwtBearer).GetValue<string>("SecurityKey");

builder.Services.AddAuthentication(x => {
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x => 
{
    x.RequireHttpsMetadata= false;
    x.SaveToken = true;
    x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtKey)),
        ValidateIssuer= false,
        ValidateAudience = false,
    };
});

//builder.Services.AddAuthentication(x => {
//    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//            .AddJwtBearer(x => {
//                var jwtConfig = new JwtConfig();

//                builder.Configuration.Bind(Constants.Sections.AuthJwtBearer, jwtConfig);

//                x.RequireHttpsMetadata = false;
//                x.SaveToken = true;
//                x.TokenValidationParameters = new TokenValidationParameters
//                {
//                    ClockSkew = TimeSpan.FromMinutes(3),
//                    ValidateIssuerSigningKey = true,
//                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtConfig.SecurityKey)),
//                    ValidateIssuer = true,
//                    ValidIssuer = jwtConfig.Issuer,
//                    ValidateLifetime = true,
//                    ValidateAudience = false
//                };
//                x.Events = new JwtBearerEvents
//                {
//                    OnAuthenticationFailed = context => {
//                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
//                        {
//                            context.Response.Headers.Add("Token-Expired", "true");
//                        }
//                        return Task.CompletedTask;
//                    }
//                };
//            })
//            ;
#endregion

// ===== Auto Mapper Configurations ====
//var mappingConfig = new MapperConfiguration(mc =>
//{
//    mc.AddProfile(new MappingProfile());
//});

//IMapper mapper = mappingConfig.CreateMapper();
//builder.Services.AddSingleton(mapper);
//builder.Services.AddAutoMapper(typeof(Program));
//builder.Services.AddFeatureManagement();
//builder.Services.AddMemoryCache();

var app = builder.Build();

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
    settings.Path = "/api";
    //settings.DocumentPath = "/api/specification.json";
});

app.UseRouting();

app.UseAuthentication();
//app.UseIdentityServer();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapRazorPages();

app.MapFallbackToFile("index.html");

app.Run();