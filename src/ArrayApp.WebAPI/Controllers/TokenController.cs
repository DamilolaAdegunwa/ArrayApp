#pragma warning disable
#pragma info disable
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using ArrayApp.Application.Common.Models;
using ArrayApp.Domain.Entities;
using ArrayApp.Infrastructure.Identity;
using ArrayApp.Infrastructure.Persistence;
using ArrayApp.Infrastructure.Services;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NSwag.Annotations;

namespace ArrayApp.WebAPI.Controllers;

[ApiExplorerSettings(IgnoreApi = false)]
[Authorize]
public class TokenController : BaseController
{
    private readonly ITokenSvc _tokenSvc;
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly IUserRoleService _userRoleService;

    public TokenController(
        ITokenSvc tokenSvc,
        ApplicationDbContext applicationDbContext,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        SignInManager<ApplicationUser> signInManager,
        IConfiguration configuration,
        IUserRoleService userRoleService
    )
    {
        _tokenSvc = tokenSvc;
        _applicationDbContext = applicationDbContext;
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _userRoleService = userRoleService;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IServiceResponse<TokenDTO>> Index([FromBody] LoginModel model)
    {
        try
        {
            Logger.LogInformation("Authentication attempt for user: {UserName}", model.UserName);
            return await HandleApiOperationAsync(async () => {

                var response = new ServiceResponse<TokenDTO>();

                var user = await _userManager.FindByNameAsync(model.UserName)
                        ?? await _userManager.FindByEmailAsync(model.UserName);

                if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    if (!user.IsConfirmed())
                    {
                        response.Code = ((int)HttpStatusCode.BadRequest).ToString();
                        response.ShortDescription = "Account not active. Please activate your account to continue.";
                        return response;
                    }

                    if (user.AccountLocked())
                    {
                        response.Code = ((int)HttpStatusCode.BadRequest).ToString();
                        response.ShortDescription = "Account locked. Please contact the system administrator.";
                        return response;
                    }

                    // Reset failed access count on successful login
                    if (user.AccessFailedCount > 0)
                    {
                        await _userManager.ResetAccessFailedCountAsync(user);
                    }

                    var roles = (await _userRoleService.GetUserRolesAsync(user.Id)).ToArray();
                    var token = GenerateToken(user, roles);

                    response.Object = token;
                    response.Code = ((int)HttpStatusCode.OK).ToString();
                    response.ShortDescription = "SUCCESS";
                }
                else
                {
                    // Increment failed access count if user exists
                    if (user != null)
                    {
                        await _userManager.AccessFailedAsync(user);
                    }

                    response.Code = ((int)HttpStatusCode.BadRequest).ToString();
                    response.ShortDescription = "Invalid credentials supplied.";
                }

                return response;
            });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Authentication failed unexpectedly: {Message}", ex.Message);
            throw;
        }
    }

    private TokenDTO GenerateToken(ApplicationUser user, string[] roles)
    {
        var jwtKey = _configuration["Authentication:JwtBearer:SecurityKey"]
                     ?? _configuration["Authentication:Schemes:Bearer:SecurityKey"]
                     ?? "ArrayAppEnterpriseSecretKeyWith256BitsMinimumLength2026!";

        // Ensure minimum 256 bits (32 bytes) for HMAC-SHA256
        var keyBytes = Encoding.UTF8.GetBytes(jwtKey);
        if (keyBytes.Length < 32)
        {
            var padded = new byte[32];
            Array.Copy(keyBytes, padded, keyBytes.Length);
            keyBytes = padded;
        }

        var key = new SymmetricSecurityKey(keyBytes);
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var durationStr = _configuration["Authentication:JwtBearer:TokenDurationInSeconds"]
                          ?? _configuration["Authentication:Schemes:Bearer:TokenDurationInSeconds"];
        var seconds = int.TryParse(durationStr, out var sec) && sec > 0 ? sec : 3600; // Default: 1 hour
        var expires = DateTimeOffset.UtcNow.AddSeconds(seconds);

        var issuer = _configuration["Authentication:JwtBearer:Issuer"]
                     ?? _configuration["Authentication:Schemes:Bearer:ValidIssuer"]
                     ?? "ArrayApp.WebUI.Issuer";

        var audience = _configuration["Authentication:JwtBearer:Audience"]
                       ?? _configuration["Authentication:Schemes:Bearer:ValidAudiences:0"]
                       ?? "ArrayApp.WebUI.Audience";

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email ?? user.UserName ?? user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim("Email", user.Email ?? string.Empty),
            new Claim("UserId", user.Id.ToString())
        };

        // Add role claims properly
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expires.UtcDateTime,
            signingCredentials: credentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return new TokenDTO { Token = tokenString, Expires = expires };
    }
}