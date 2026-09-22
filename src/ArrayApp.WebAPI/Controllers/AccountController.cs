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

[Authorize]
public class AccountController : BaseController
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly IServiceHelper _serviceHelper;
    private readonly IAccountService _accountService;
    private readonly IConfiguration _configuration;

    public AccountController(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        ApplicationDbContext applicationDbContext,
        IServiceHelper serviceHelper,
        IAccountService accountService,
        IConfiguration configuration
    )
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _roleManager = roleManager;
        _applicationDbContext = applicationDbContext;
        _serviceHelper = serviceHelper;
        _accountService = accountService;
        _configuration = configuration;
    }

    private static UserDto? MapToUserDto(ApplicationUser? user)
    {
        if (user == null) return null;
        return new UserDto
        {
            Id = user.Id.ToString(),
            UserName = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            EmailConfirmed = user.EmailConfirmed,
            PhoneNumber = user.PhoneNumber ?? string.Empty,
            PhoneNumberConfirmed = user.PhoneNumberConfirmed,
            TwoFactorEnabled = user.TwoFactorEnabled,
            LockoutEnabled = user.LockoutEnabled,
            LockoutEnd = user.LockoutEnd,
            AccessFailedCount = user.AccessFailedCount
        };
    }

    [HttpGet]
    [Route("ping")]
    public IActionResult Ping()
    {
        return Ok("the account controller was reached!");
    }

    [HttpGet]
    [Route("GetProfile")]
    public async Task<IServiceResponse<UserDto>> GetCurrentUserProfile()
    {
        return await HandleApiOperationAsync(async () => {
            var response = new ServiceResponse<UserDto>();
            var name = User.FindFirst(JwtClaimTypes.Name)?.Value 
                       ?? User.FindFirst(ClaimTypes.Name)?.Value
                       ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            var profile = await _userManager.FindByNameAsync(name ?? string.Empty);
            response.Object = MapToUserDto(profile);
            return response;
        });
    }

    [AllowAnonymous]
    [HttpPost]
    [Route("SignUp")]
    public async Task<IServiceResponse<bool>> SignUp([FromBody] LoginModel loginModel)
    {
        return await HandleApiOperationAsync(async () => {
            await _accountService.SignUp(loginModel);
            return new ServiceResponse<bool>(true);
        });
    }

    [HttpPost]
    [Route("AddUser")]
    [Authorize(Roles = "Administrator,Admin,admin")]
    public async Task<IServiceResponse<bool>> AddUser([FromBody] LoginModel model)
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
    public async Task<IServiceResponse<List<ClaimDto>>> GetClaims()
    {
        return await HandleApiOperationAsync(async () => {
            var response = new ServiceResponse<List<ClaimDto>>();
            response.Object = User.Claims.Select(c => new ClaimDto { 
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
        return await HandleApiOperationAsync(async () => {
            var response = new ServiceResponse<List<ClaimsIdentityDto>>();
            response.Object = User.Identities.Select(c => new ClaimsIdentityDto
            {
                Actor = new ClaimsIdentityActorDto() { 
                    AuthenticationType = c?.Actor?.AuthenticationType,
                    IsAuthenticated = c?.Actor?.IsAuthenticated ?? false,
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
                IsAuthenticated = c?.IsAuthenticated ?? false,
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
    [Authorize(Roles = "Administrator,Admin,admin,manager")]
    public async Task<IServiceResponse<List<UserDto>>> GetAllUsers()
    {
        return await HandleApiOperationAsync(async () => {
            var response = new ServiceResponse<List<UserDto>>();
            var users = await _userManager.Users.ToListAsync();
            response.Object = users.Select(MapToUserDto).Where(u => u != null).Select(u => u!).ToList();
            return response;
        });
    }

    [HttpGet]
    [Route("GetUserById/{userId}")]
    public async Task<ServiceResponse<UserDto>> GetUserById(string userId)
    {
        return await HandleApiOperationAsync(async () => {
            var response = new ServiceResponse<UserDto>();
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                response.ShortDescription = "User not found";
                return response;
            }

            response.Object = MapToUserDto(user);
            return response;
        });
    }

    [HttpGet]
    [Route("GetUserByEmail/{email}")]
    public async Task<ServiceResponse<UserDto>> GetUserByEmail(string email)
    {
        return await HandleApiOperationAsync(async () => {
            var response = new ServiceResponse<UserDto>();
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                response.ShortDescription = "User not found";
                return response;
            }

            response.Object = MapToUserDto(user);
            return response;
        });
    }

    [HttpGet]
    [Route("GetUserByUsername/{username}")]
    public async Task<IServiceResponse<UserDto>> GetUserByUsername(string username)
    {
        return await HandleApiOperationAsync(async () => {
            var response = new ServiceResponse<UserDto>();
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                response.ShortDescription = "User not found";
                return response;
            }

            response.Object = MapToUserDto(user);
            return response;
        });
    }

    [HttpGet]
    [Route("GetAllRoles")]
    [Authorize(Roles = "Administrator,Admin,admin,manager")]
    public async Task<IServiceResponse<List<ApplicationRole>>> GetAllRoles()
    {
        return await HandleApiOperationAsync(async () => {
            var response = new ServiceResponse<List<ApplicationRole>>();
            var roles = await _roleManager.Roles.ToListAsync();

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
    [Authorize(Roles = "Administrator,Admin,admin")]
    public async Task<ServiceResponse<bool>> ResetRoles([FromBody] ResetRolesModel model)
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
    public async Task<ServiceResponse<bool>> ResetPassword([FromBody] ResetPasswordModel model)
    {
        return await HandleApiOperationAsync(async () => {
            var response = new ServiceResponse<bool>();

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) 
                                ?? User.FindFirst("UserId")?.Value 
                                ?? User.FindFirst("sub")?.Value;
            var isAdmin = User.IsInRole("Administrator") || User.IsInRole("Admin") || User.IsInRole("admin");

            // Prevent BOLA: Non-admins cannot reset another user's password
            if (!isAdmin && !string.Equals(currentUserId, model.UserId, StringComparison.OrdinalIgnoreCase))
            {
                response.ShortDescription = "Unauthorized: You may only reset your own password.";
                response.Code = "403";
                return response;
            }

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                response.ShortDescription = "User not found";
                return response;
            }

            IdentityResult result;
            if (!string.IsNullOrEmpty(model.ResetToken))
            {
                // Verify provided reset token
                result = await _userManager.ResetPasswordAsync(user, model.ResetToken, model.NewPassword);
            }
            else if (!string.IsNullOrEmpty(model.CurrentPassword))
            {
                // Verify current password
                result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            }
            else if (isAdmin)
            {
                // Administrative reset generates token explicitly with administrative authorization
                var adminResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                result = await _userManager.ResetPasswordAsync(user, adminResetToken, model.NewPassword);
            }
            else
            {
                response.ShortDescription = "A valid ResetToken or CurrentPassword is required to reset password.";
                response.Code = "400";
                return response;
            }

            if (!result.Succeeded)
            {
                response.ShortDescription = string.Join(", ", result.Errors.Select(e => e.Description));
                return response;
            }

            response.Object = true;
            return response;
        });
    }

    [HttpPost]
    [Route("ChangePassword")]
    public async Task<ServiceResponse<bool>> ChangePassword([FromBody] ChangePasswordModel model)
    {
        return await HandleApiOperationAsync(async () => {
            var response = new ServiceResponse<bool>();

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) 
                                ?? User.FindFirst("UserId")?.Value 
                                ?? User.FindFirst("sub")?.Value;
            var isAdmin = User.IsInRole("Administrator") || User.IsInRole("Admin") || User.IsInRole("admin");

            if (!isAdmin && !string.Equals(currentUserId, model.UserId, StringComparison.OrdinalIgnoreCase))
            {
                response.ShortDescription = "Unauthorized: You may only change your own password.";
                response.Code = "403";
                return response;
            }

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                response.ShortDescription = "User not found";
                return response;
            }

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                response.ShortDescription = string.Join(", ", result.Errors.Select(e => e.Description));
                return response;
            }

            response.Object = true;
            return response;
        });
    }

    [HttpPost]
    [Route("SendResetOTP")]
    [AllowAnonymous]
    public async Task<ServiceResponse<bool>> SendResetOTP([FromBody] SendResetOTPModel model)
    {
        return await HandleApiOperationAsync(async () => {
            var response = new ServiceResponse<bool>();

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Generic response to avoid user enumeration
                response.Object = true;
                return response;
            }

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _accountService.SendResetPasswordOTP(model.Email, resetToken);
            response.Object = true;
            return response;
        });
    }

    [HttpPost]
    [Route("AddRolesToUser")]
    [Authorize(Roles = "Administrator,Admin,admin")]
    public async Task<ServiceResponse<bool>> AddRolesToUser([FromBody] AddRolesToUserModel model)
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
                response.ShortDescription = string.Join(", ", result.Errors.Select(e => e.Description));
                return response;
            }

            response.Object = true;
            return response;
        });
    }

    [HttpPost]
    [Route("RemoveRolesFromUser")]
    [Authorize(Roles = "Administrator,Admin,admin")]
    public async Task<ServiceResponse<bool>> RemoveRolesFromUser([FromBody] RemoveRolesFromUserModel model)
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
                response.ShortDescription = string.Join(", ", result.Errors.Select(e => e.Description));
                return response;
            }

            response.Object = true;
            return response;
        });
    }

    [HttpPost("roles")]
    [Authorize(Roles = "Administrator,Admin,admin")]
    public async Task<IActionResult> CreateRole([FromBody] RoleViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var role = new ApplicationRole { Name = model.Name };
        var result = await _roleManager.CreateAsync(role);

        if (result.Succeeded)
        {
            return Ok(new { Message = "Role created successfully" });
        }
        else
        {
            return BadRequest(new { Message = "Failed to create role", Errors = result.Errors });
        }
    }

    [HttpGet("roles")]
    [Authorize(Roles = "Administrator,Admin,admin,manager")]
    public async Task<IActionResult> GetAllRolesV2()
    {
        var roles = await _roleManager.Roles.ToListAsync();
        return Ok(roles);
    }

    [HttpGet("roles/{roleName}")]
    [Authorize(Roles = "Administrator,Admin,admin,manager")]
    public async Task<IActionResult> GetRoleByName(string roleName)
    {
        var role = await _roleManager.FindByNameAsync(roleName);

        if (role != null)
        {
            return Ok(role);
        }
        else
        {
            return NotFound();
        }
    }

    [HttpDelete("roles/{roleName}")]
    [Authorize(Roles = "Administrator,Admin,admin")]
    public async Task<IActionResult> DeleteRole(string roleName)
    {
        var role = await _roleManager.FindByNameAsync(roleName);

        if (role != null)
        {
            var result = await _roleManager.DeleteAsync(role);

            if (result.Succeeded)
            {
                return Ok(new { Message = "Role deleted successfully" });
            }
            else
            {
                return BadRequest(new { Message = "Failed to delete role", Errors = result.Errors });
            }
        }
        else
        {
            return NotFound();
        }
    }

    [HttpGet("roles/search")]
    [Authorize(Roles = "Administrator,Admin,admin,manager")]
    public async Task<IActionResult> SearchRoles(
        string search = "",
        int page = 1,
        int pageSize = 10,
        string orderBy = "Name",
        bool ascending = true)
    {
        IQueryable<ApplicationRole> query = _roleManager.Roles;

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(r => r.Name.Contains(search));
        }

        if (ascending)
        {
            query = query.OrderBy(r => r.Name);
        }
        else
        {
            query = query.OrderByDescending(r => r.Name);
        }

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        var roles = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return Ok(new
        {
            Roles = roles,
            TotalCount = totalCount,
            TotalPages = totalPages,
            CurrentPage = page
        });
    }

    [HttpGet("users/{userId}/roles")]
    public async Task<IActionResult> GetUserRoles(string userId)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) 
                            ?? User.FindFirst("UserId")?.Value 
                            ?? User.FindFirst("sub")?.Value;
        var isAdmin = User.IsInRole("Administrator") || User.IsInRole("Admin") || User.IsInRole("admin");

        if (!isAdmin && !string.Equals(currentUserId, userId, StringComparison.OrdinalIgnoreCase))
        {
            return Forbid();
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            return Ok(userRoles);
        }
        else
        {
            return NotFound();
        }
    }

    [HttpGet("roles/{roleName}/users")]
    [Authorize(Roles = "Administrator,Admin,admin,manager")]
    public async Task<IActionResult> GetUsersInRole(string roleName)
    {
        var usersInRole = await _userManager.GetUsersInRoleAsync(roleName);
        return Ok(usersInRole.Select(MapToUserDto).Where(u => u != null).ToList());
    }

    [HttpGet("users/roles/any")]
    [Authorize(Roles = "Administrator,Admin,admin,manager")]
    public async Task<IActionResult> GetUsersInAnyRole([FromQuery] string[] roleNames)
    {
        var matchedUsers = new List<ApplicationUser>();
        foreach (var role in roleNames)
        {
            var users = await _userManager.GetUsersInRoleAsync(role);
            matchedUsers.AddRange(users);
        }

        var distinctUsers = matchedUsers.GroupBy(u => u.Id).Select(g => g.First()).Select(MapToUserDto).ToList();
        return Ok(distinctUsers);
    }

    [HttpGet("users/roles/all")]
    [Authorize(Roles = "Administrator,Admin,admin,manager")]
    public async Task<IActionResult> GetUsersInAllRoles([FromQuery] string[] roleNames)
    {
        if (roleNames == null || roleNames.Length == 0)
        {
            return Ok(new List<UserDto>());
        }

        var firstRoleUsers = await _userManager.GetUsersInRoleAsync(roleNames[0]);
        var candidateUsers = new List<ApplicationUser>(firstRoleUsers);

        for (int i = 1; i < roleNames.Length; i++)
        {
            var roleUsers = (await _userManager.GetUsersInRoleAsync(roleNames[i])).Select(u => u.Id).ToHashSet();
            candidateUsers.RemoveAll(u => !roleUsers.Contains(u.Id));
        }

        return Ok(candidateUsers.Select(MapToUserDto).ToList());
    }
}