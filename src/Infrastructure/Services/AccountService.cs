using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ArrayApp.Application.Common.Models;
using ArrayApp.Domain.Common;
using ArrayApp.Infrastructure.Identity;
using ArrayApp.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using Microsoft.EntityFrameworkCore;
using ArrayApp.Domain.Entities;
using ArrayApp.Domain.Enums;

namespace ArrayApp.Infrastructure.Services;
public interface IAccountService
{
    Task<bool> AddUser(LoginModel model, string username);
    Task SignUp(LoginModel model);

    Task<bool> SendResetPasswordOTP(string username, string otp);
}
public class AccountService : IAccountService
{
    private readonly ILogger log = Log.Logger;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly IHostingEnvironment _hostingEnvironment;
    private readonly IServiceHelper _svcHelper;
    //private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType.Name);
    public AccountService(
         //ILogger<AccountService> logger
         SignInManager<ApplicationUser> signInManager
         ,UserManager<ApplicationUser> userManager
         ,RoleManager<ApplicationRole> roleManager
        , ApplicationDbContext applicationDbContext
        , IHostingEnvironment hostingEnvironment
        , IServiceHelper serviceHelper

        )
    {
        //log = logger;
        _signInManager = signInManager;
        _userManager = userManager;
        _roleManager = roleManager;
        _applicationDbContext = applicationDbContext;
        _hostingEnvironment = hostingEnvironment;
        _svcHelper = serviceHelper;
    }
    public async Task<bool> AddUser(LoginModel model, string username)
    {
        try
        {
            #region validate credential

            //check that the model carries data
            if (model == null)
            {
                throw new Exception("no input provided!");
            }
            //check for non-empty username 
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new Exception("Please login and retry");
            }

            //check that the user exist
            var user = _applicationDbContext.Users.FirstOrDefault(x => x.UserName == username);
            if (user == null)
            {
                throw new Exception("User does not exist");
            }
            //check that the person is an administrator
            if (user.UserType != UserType.Administrator)
            {
                throw new Exception("you are not authorized to add user");
            }

            //check for valid usertype, validate the adtype if premium whether user can put premium ad
            #endregion
            user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.UserName,
                AccountConfirmationCode = CommonHelper.GenerateRandonAlphaNumeric(),
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                UserType = model.UserType,
            };
            var password = CommonHelper.GenerateRandonAlphaNumeric(); //"password";
            var creationStatus = await _userManager.CreateAsync(user, password);

            if (!creationStatus.Succeeded)
            {
                throw new Exception(creationStatus.Errors?.FirstOrDefault().Description);
            }

            //you'd need to email the account credentials to the new user...
            try
            {
                //first get the file
                var filePath = Path.Combine(_hostingEnvironment.ContentRootPath, Constants.Url.WelcomeEmail);
                if (File.Exists(filePath))
                {
                    var fileString = File.ReadAllText(filePath);
                    if (!string.IsNullOrWhiteSpace(fileString))
                    {
                        var admin = "";
                        if (model.UserType == UserType.Administrator) { admin = "(Administrator)"; }
                        fileString = fileString.Replace("{{UserName}}", $"{model.UserName} {admin}");
                        fileString = fileString.Replace("{{DefaultPassword}}", $"{password}");

                        _svcHelper.SendEMail(model.UserName, fileString, "Seven Peaks Software Co., Ltd: You are welcome on board!");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"{ex.Message} :: {MethodBase.GetCurrentMethod().Name} :: {ex.StackTrace} ");
            }
            return true;
        }
        catch (Exception ex)
        {
            Log.Error($"{ex.Message} :: {MethodBase.GetCurrentMethod().Name} :: {ex.StackTrace} ");
            //return false;
            throw ex;
        }
    }

    public async Task SignUp(LoginModel model)
    {
        #region validate credential

        //check that the model carries data
        if (model == null)
        {
            throw new Exception("Invalid parameter");
        }
        //check that the model carries a password 
        if (string.IsNullOrWhiteSpace(model.Password))
        {
            throw new Exception("Please input a password");
        }

        //check that the user does not already exist
        var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == model.UserName);
        if (user != null)
        {
            throw new Exception("User already exist");
        }

        //check that the username is a valid email ( the password would be validate by the Identity builder)
        if (!Regex.IsMatch(model.UserName, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase))
        {
            throw new Exception("The UserName isn't Invalid Email");
        }

        //check for validate usertype
        #endregion

        #region sign up a new user
        try
        {
            user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.UserName,
                AccountConfirmationCode = CommonHelper.GenerateRandonAlphaNumeric(),
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                UserType = model.UserType,
            };
            var password = model.Password;// "password";
            var creationStatus = await _userManager.CreateAsync(user, password);

            if (creationStatus.Succeeded)
            {
                try
                {
                    //first get the file
                    var filePath = Path.Combine(_hostingEnvironment.ContentRootPath, Constants.Url.WelcomeEmail);
                    if (File.Exists(filePath))
                    {
                        var fileString = File.ReadAllText(filePath);
                        if (!string.IsNullOrWhiteSpace(fileString))
                        {
                            var admin = "";
                            if (model.UserType == UserType.Administrator) { admin = "(Administrator)"; }
                            fileString = fileString.Replace("{{UserName}}", $"{model.UserName} {admin}");
                            fileString = fileString.Replace("{{DefaultPassword}}", $"{password}");

                            _svcHelper.SendEMail(model.UserName, fileString, "Seven Peaks Software Co., Ltd: You are welcome on board!");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"{ex.Message} :: {MethodBase.GetCurrentMethod().Name} :: {ex.StackTrace}");
                }
            }
            else
            {
                throw new Exception(creationStatus.Errors.FirstOrDefault()?.Description);
            }
        }
        catch (Exception ex)
        {
            var errMsg = $"an error occured while trying to signup. Please try again!";
            Log.Error($"{errMsg} :: stack trace - {ex.StackTrace} :: exception message - {ex.Message}", ex);
            throw new Exception(errMsg);
        }
        #endregion
    }

    public async Task<bool> SendResetPasswordOTP(string username, string otp)
    {
        //you'd need to email the account user the otp...
        try
        {
            //first get the file
            var filePath = Path.Combine(_hostingEnvironment.ContentRootPath, Constants.Url.ResetPasswordOTP);
            if (File.Exists(filePath))
            {
                var fileString = File.ReadAllText(filePath);
                if (!string.IsNullOrWhiteSpace(fileString))
                {
                    //var admin = "";
                    //if (model.UserType == UserType.Administrator) { admin = "(Administrator)"; }
                    fileString = fileString.Replace("{{UserName}}", $"{username}");
                    fileString = fileString.Replace("{{OTP}}", $"{otp}");

                    _svcHelper.SendEMail(username, fileString, Constants.Url.ResetPasswordSubject);
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error($"{ex.Message} :: {MethodBase.GetCurrentMethod().Name} :: {ex.StackTrace} ");
            return false;
        }
        return true;
    }
}