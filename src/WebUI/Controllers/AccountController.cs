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
namespace ArrayApp.WebUI.Controllers;
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
    public AccountController(
        //ILogger<AccountController> logger
         SignInManager<ApplicationUser> signInManager
        , UserManager<ApplicationUser> userManager
        , RoleManager<ApplicationRole> roleManager
        , ApplicationDbContext applicationDbContext
        , IServiceHelper serviceHelper
        , IAccountService accountService
        ) //: base(logger)
    {
        //_logger = logger;
        _signInManager = signInManager;
        _userManager = userManager;
        _roleManager = roleManager;
        _applicationDbContext = applicationDbContext;
        _serviceHelper = serviceHelper;
        _accountService = accountService;
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
    public async Task<ServiceResponse<bool>> AddUser(LoginModel model)
    {
        return await HandleApiOperationAsync(async () => {
            var response = new ServiceResponse<bool>();
            var data = await _accountService.AddUser(model, _serviceHelper.GetCurrentUserEmail());
            response.Object = data;
            return response;
        });
    }
}