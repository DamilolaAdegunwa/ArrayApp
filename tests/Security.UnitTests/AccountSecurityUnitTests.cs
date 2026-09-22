using System.Reflection;
using System.Security.Claims;
using ArrayApp.Application.Common.Models;
using ArrayApp.Domain.Entities;
using ArrayApp.Infrastructure.Identity;
using ArrayApp.Infrastructure.Persistence;
using ArrayApp.Infrastructure.Services;
using ArrayApp.WebAPI.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;

namespace ArrayApp.Security.UnitTests;

[TestFixture]
public class AccountSecurityUnitTests
{
    private Mock<UserManager<ApplicationUser>> _mockUserManager;
    private Mock<SignInManager<ApplicationUser>> _mockSignInManager;
    private Mock<RoleManager<ApplicationRole>> _mockRoleManager;
    private Mock<IServiceHelper> _mockServiceHelper;
    private Mock<IAccountService> _mockAccountService;
    private Mock<IConfiguration> _mockConfiguration;

    [SetUp]
    public void SetUp()
    {
        var userStore = new Mock<IUserStore<ApplicationUser>>();
        _mockUserManager = new Mock<UserManager<ApplicationUser>>(userStore.Object, null, null, null, null, null, null, null, null);

        var contextAccessor = new Mock<IHttpContextAccessor>();
        var userPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
        _mockSignInManager = new Mock<SignInManager<ApplicationUser>>(_mockUserManager.Object, contextAccessor.Object, userPrincipalFactory.Object, null, null, null, null);

        var roleStore = new Mock<IRoleStore<ApplicationRole>>();
        _mockRoleManager = new Mock<RoleManager<ApplicationRole>>(roleStore.Object, null, null, null, null);

        _mockServiceHelper = new Mock<IServiceHelper>();
        _mockAccountService = new Mock<IAccountService>();
        _mockConfiguration = new Mock<IConfiguration>();
    }

    [Test]
    public void RoleManagementEndpoints_MustRequire_AdministratorRole()
    {
        var methods = typeof(AccountController).GetMethods(BindingFlags.Public | BindingFlags.Instance);
        var sensitiveEndpoints = new[] { "ResetRoles", "AddRolesToUser", "RemoveRolesFromUser", "CreateRole", "DeleteRole" };

        foreach (var endpointName in sensitiveEndpoints)
        {
            var method = methods.FirstOrDefault(m => m.Name == endpointName);
            method.Should().NotBeNull($"Endpoint {endpointName} must exist on AccountController");

            var authAttr = method!.GetCustomAttribute<AuthorizeAttribute>();
            authAttr.Should().NotBeNull($"Endpoint {endpointName} must be protected with [Authorize]");
            authAttr!.Roles.Should().Contain("Administrator", $"Endpoint {endpointName} must require Administrator role to prevent vertical privilege escalation");
        }
    }

    [Test]
    public async Task ResetPassword_WhenCalledByNonAdminForDifferentUser_MustReturnForbiddenBOLAError()
    {
        var controller = new AccountController(
            _mockSignInManager.Object,
            _mockUserManager.Object,
            _mockRoleManager.Object,
            null!,
            _mockServiceHelper.Object,
            _mockAccountService.Object,
            _mockConfiguration.Object
        );

        // Caller identity: User 100 (Standard User)
        var userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "100"),
            new Claim("UserId", "100"),
            new Claim(ClaimTypes.Role, "StandardUser")
        }, "TestAuth"));

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = userPrincipal }
        };

        // Attempting to reset password of User 200 (Victim / Admin)
        var model = new ResetPasswordModel
        {
            UserId = "200",
            NewPassword = "AttackerPassword123!"
        };

        var response = await controller.ResetPassword(model);

        response.Code.Should().Be("403", "Cross-user password reset by non-admin must be rejected with 403 Forbidden");
        response.ShortDescription.Should().Contain("Unauthorized", "Must explicitly report unauthorized reset attempt");
        _mockUserManager.Verify(u => u.ResetPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task ResetPassword_WhenCalledWithoutResetTokenOrCurrentPassword_MustBeRejected()
    {
        var controller = new AccountController(
            _mockSignInManager.Object,
            _mockUserManager.Object,
            _mockRoleManager.Object,
            null!,
            _mockServiceHelper.Object,
            _mockAccountService.Object,
            _mockConfiguration.Object
        );

        // Caller identity: User 100 resetting their own password
        var userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "100"),
            new Claim("UserId", "100"),
            new Claim(ClaimTypes.Role, "StandardUser")
        }, "TestAuth"));

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = userPrincipal }
        };

        _mockUserManager.Setup(u => u.FindByIdAsync("100"))
            .ReturnsAsync(new ApplicationUser { Id = 100, UserName = "alice" });

        // Missing both ResetToken and CurrentPassword
        var model = new ResetPasswordModel
        {
            UserId = "100",
            NewPassword = "NewSecretPassword123!",
            ResetToken = null,
            CurrentPassword = null
        };

        var response = await controller.ResetPassword(model);

        response.Code.Should().Be("400");
        response.ShortDescription.Should().Contain("ResetToken or CurrentPassword is required");
        _mockUserManager.Verify(u => u.ResetPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
}
