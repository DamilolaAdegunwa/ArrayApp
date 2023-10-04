using ArrayApp.Infrastructure.Persistence;
using ArrayApp.Infrastructure.Services;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using ArrayApp.Infrastructure.Identity;
using ArrayApp.Application.Common.Models;
using ArrayApp.Domain.Entities;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace ArrayApp.WebAPI.Controllers;
//public class AccountController : Controller
//{
//    public IActionResult Index()
//    {
//        return View();
//    }
//}
[Authorize]
public class AccountController : BaseController
{
    #region main account endpoints
    //private readonly ILogger<AccountController> _logger;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly IServiceHelper _serviceHelper;
    private readonly IAccountService _accountService;
    public readonly IConfiguration _configuration;
    public AccountController(
        //ILogger<AccountController> logger
         SignInManager<ApplicationUser> signInManager
        , UserManager<ApplicationUser> userManager
        , RoleManager<ApplicationRole> roleManager
        , ApplicationDbContext applicationDbContext
        , IServiceHelper serviceHelper
        , IAccountService accountService
        , IConfiguration configuration
        ) //: base(logger)
    {
        //_logger = logger;
        _signInManager = signInManager;
        _userManager = userManager;
        _roleManager = roleManager;
        _applicationDbContext = applicationDbContext;
        _serviceHelper = serviceHelper;
        _accountService = accountService;
        _configuration = configuration;
    }

    [HttpGet]
    [Route("GetProfile")]
    public async Task<IServiceResponse<ApplicationUser>> GetCurrentUserProfile()
    {
        return await HandleApiOperationAsync(async () => {

            var response = new ServiceResponse<ApplicationUser>();

            var name = User.FindFirst(JwtClaimTypes.Name)?.Value;

            var profile = await _userManager.FindByNameAsync(name);
            response.Object = profile;
            return response;
        });
    }

    [AllowAnonymous]
    [HttpPost]
    [Route("SignUp")]/*SignUp - Create your account */
    public async Task<IServiceResponse<bool>> SignUp(LoginModel loginModel)
    {
        return await HandleApiOperationAsync(async () => {
            await _accountService.SignUp(loginModel);
            return new ServiceResponse<bool>(true);
        });
    }


    #endregion

    [HttpPost]
    [Route("AddUser")]
    public async Task<IServiceResponse<bool>> AddUser(LoginModel model)
    {
        return await HandleApiOperationAsync(async () => {
            var response = new ServiceResponse<bool>();
            var data = await _accountService.AddUser(model, _serviceHelper.GetCurrentUserEmail());
            response.Object = data;
            return response;
        });
    }

    [HttpGet]
    [Route("GetClaims")]
    //public async Task<IServiceResponse<ClaimsPrincipal>> GetIdentity()
    public async Task<IServiceResponse<List<ClaimDto>>> GetClaims()
    {
        //.Claims.ToList()[0].
        //var x = new ClaimsPrincipal().Claims.ToList();
        return await HandleApiOperationAsync(async () => {
            var response = new ServiceResponse<List<ClaimDto>>();
            response.Object = User.Claims.ToList<Claim>().Select(c => new ClaimDto { 
                Issuer = c.Issuer,
                OriginalIssuer = c.OriginalIssuer,
                Properties = c.Properties,
                Type = c.Type,
                Value = c.Value,
                ValueType = c.ValueType
            }).ToList();
            return response;
        });
    }

    [HttpGet]
    [Route("GetClaimsIdentity")]
    public async Task<IServiceResponse<List<ClaimsIdentityDto>>> ClaimsIdentity()
    {
        //var x = User.Identities.ToList()[0].IsAuthenticated;
        //.Claims.ToList()[0].
        //var x = new ClaimsPrincipal().Claims.ToList();
        return await HandleApiOperationAsync(async () => {
            var response = new ServiceResponse<List<ClaimsIdentityDto>>();
            response.Object = User.Identities.ToList().Select(c => new ClaimsIdentityDto
            {
                Actor = new ClaimsIdentityActorDto() { 
                    AuthenticationType = c?.Actor?.AuthenticationType,
                    IsAuthenticated = c?.Actor?.IsAuthenticated??false,
                    BootstrapContext = c?.Actor?.BootstrapContext,
                    Claims = c?.Actor?.Claims.Select(d => new ClaimDto {
                        Issuer = d.Issuer,
                        OriginalIssuer = d.OriginalIssuer,
                        Properties = d.Properties,
                        Type = d.Type,
                        Value = d.Value,
                        ValueType = d.ValueType
                    }).ToList(),
                    Label = c?.Actor?.Label,
                    Name = c?.Actor?.Name,
                    NameClaimType = c?.Actor?.NameClaimType,
                    RoleClaimType = c?.Actor?.RoleClaimType,
                    
                },
                AuthenticationType = c?.AuthenticationType,
                IsAuthenticated = c?.IsAuthenticated??false,
                BootstrapContext = c?.BootstrapContext,
                Claims = c?.Claims.Select(e => new ClaimDto
                {
                    Issuer = e.Issuer,
                    OriginalIssuer = e.OriginalIssuer,
                    Properties = e.Properties,
                    Type = e.Type,
                    Value = e.Value,
                    ValueType = e.ValueType
                }).ToList(),
                Label = c?.Label,
                Name = c?.Label,
                NameClaimType = c?.NameClaimType,
                RoleClaimType = c?.RoleClaimType,
                
            }).ToList();
            return response;
        });
    }

    [HttpGet]
    [Route("GetAllUsers")]
    public async Task<IServiceResponse<List<ApplicationUser>>> GetAllUsers()
    {
        return await HandleApiOperationAsync(async () => {
            var response = new ServiceResponse<List<ApplicationUser>>();
            var users = await _userManager.Users.ToListAsync();
            response.Object = users;
            return response;
        });
    }

    [HttpGet]
    [Route("GetUserById/{userId}")]
    public async Task<ServiceResponse<ApplicationUser>> GetUserById(string userId)
    {
        return await HandleApiOperationAsync(async () => {
            var response = new ServiceResponse<ApplicationUser>();
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                response.ShortDescription = "User not found";
                return response;
            }

            response.Object = user;
            return response;
        });
    }

    [HttpGet]
    [Route("GetUserByEmail/{email}")]
    public async Task<ServiceResponse<ApplicationUser>> GetUserByEmail(string email)
    {
        return await HandleApiOperationAsync(async () => {
            var response = new ServiceResponse<ApplicationUser>();
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                response.ShortDescription = "User not found";
                return response;
            }

            response.Object = user;
            return response;
        });
    }

    [HttpGet]
    [Route("GetUserByUsername/{username}")]
    public async Task<IServiceResponse<ApplicationUser>> GetUserByUsername(string username)
    {
        return await HandleApiOperationAsync(async () => {
            var response = new ServiceResponse<ApplicationUser>();
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                response.ShortDescription = "User not found";
                return response;
            }

            response.Object = user;
            return response;
        });
    }

    [HttpGet]
    [Route("GetAllRoles")]
    public async Task<IServiceResponse<List<ApplicationRole>>> GetAllRoles()
    {
        return await HandleApiOperationAsync(async () => {
            var response = new ServiceResponse<List<ApplicationRole>>();
            var roles = _roleManager.Roles.ToList();//.Select(role => role.Name).ToList();

            if (roles == null)
            {
                response.ShortDescription = "roles not found";
                return response;
            }

            response.Object = roles;
            return response;
        });
    }

    [HttpPost]
    [Route("ResetRoles")]
    public async Task<ServiceResponse<bool>> ResetRoles(ResetRolesModel model)
    {
        return await HandleApiOperationAsync(async () => {
            var response = new ServiceResponse<bool>();

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                response.ShortDescription = "User not found";
                return response;
            }

            var existingRoles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, existingRoles);
            if (!result.Succeeded)
            {
                response.ShortDescription = "Failed to reset roles for the user";
                return response;
            }

            var rolesToAdd = model.RoleNames.Distinct();
            result = await _userManager.AddToRolesAsync(user, rolesToAdd);
            if (!result.Succeeded)
            {
                response.ShortDescription = "Failed to assign new roles to the user";
                return response;
            }

            response.Object = true;
            return response;
        });
    }
    [HttpPost]
    [Route("ResetPassword")]
    public async Task<ServiceResponse<bool>> ResetPassword(ResetPasswordModel model)
    {
        return await HandleApiOperationAsync(async () => {
            var response = new ServiceResponse<bool>();

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                response.ShortDescription = "User not found";
                return response;
            }

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, resetToken, model.NewPassword);
            if (!result.Succeeded)
            {
                response.ShortDescription = "Failed to reset password for the user";
                return response;
            }

            response.Object = true;
            return response;
        });
    }
    [HttpPost]
    [Route("ChangePassword")]
    public async Task<ServiceResponse<bool>> ChangePassword(ChangePasswordModel model)
    {
        return await HandleApiOperationAsync(async () => {
            var response = new ServiceResponse<bool>();

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                response.ShortDescription = "User not found";
                return response;
            }

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                response.ShortDescription = "Failed to change password for the user";
                return response;
            }

            response.Object = true;
            return response;
        });
    }
    [HttpPost]
    [Route("SendResetOTP")]
    public async Task<ServiceResponse<bool>> SendResetOTP(SendResetOTPModel model)
    {
        return await HandleApiOperationAsync(async () => {
            var response = new ServiceResponse<bool>();

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                response.ShortDescription = "User not found";
                return response;
            }

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            // TODO: Send reset OTP (e.g., via email or SMS)
            var resp = await _accountService.SendResetPasswordOTP(model.Email, resetToken);
            response.Object = true;
            return response;
        });
    }
    [HttpPost]
    [Route("AddRolesToUser")]
    public async Task<ServiceResponse<bool>> AddRolesToUser(AddRolesToUserModel model)
    {
        return await HandleApiOperationAsync(async () => {
            var response = new ServiceResponse<bool>();

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                response.ShortDescription = "User not found";
                return response;
            }

            var rolesToAdd = model.RoleNames.Distinct();
            var result = await _userManager.AddToRolesAsync(user, rolesToAdd);
            if (!result.Succeeded)
            {
                response.ShortDescription = "Failed to add roles to the user";
                return response;
            }

            response.Object = true;
            return response;
        });
    }
    [HttpPost]
    [Route("RemoveRolesFromUser")]
    public async Task<ServiceResponse<bool>> RemoveRolesFromUser(RemoveRolesFromUserModel model)
    {
        return await HandleApiOperationAsync(async () => {
            var response = new ServiceResponse<bool>();

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                response.ShortDescription = "User not found";
                return response;
            }

            var rolesToRemove = model.RoleNames.Distinct();
            var result = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
            if (!result.Succeeded)
            {
                response.ShortDescription = "Failed to remove roles from the user";
                return response;
            }

            response.Object = true;
            return response;
        });
    }

}