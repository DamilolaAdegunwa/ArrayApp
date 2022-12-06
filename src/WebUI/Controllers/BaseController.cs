using System.Net;
using System.Runtime.CompilerServices;
using ArrayApp.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Serilog;
namespace ArrayApp.WebUI.Controllers;
//public class BaseController : Controller
//{
//    public IActionResult Index()
//    {
//        return View();
//    }
//}
[Route("api/[controller]")]
[ApiController]
public class BaseController : ApiControllerBase//: ControllerBase
{
    //private readonly ILogger _logger = 
    public BaseController(/*ILogger logger*/)
    {
        //_logger = logger;
    }

    protected async Task<ServiceResponse<T>> HandleApiOperationAsync<T>(
   Func<Task<ServiceResponse<T>>> action, [CallerLineNumber] int lineNo = 0, [CallerMemberName] string method = "")
    {
        //var _logger = LogManager.GetLogger(typeof(BaseController));

        Log.Information($"ENTERS ({method}) method");

        var serviceResponse = new ServiceResponse<T>
        {
            Code = ((int)HttpStatusCode.OK).ToString(),
            ShortDescription = "SUCCESS"
        };

        try
        {
            if (!ModelState.IsValid)
            {
                throw new Exception("There were errors in your input, please correct them and try again.");
            }
            var actionResponse = await action();

            serviceResponse.Object = actionResponse.Object;
            serviceResponse.ShortDescription = actionResponse.ShortDescription ?? serviceResponse.ShortDescription;
            serviceResponse.Code = actionResponse.Code ?? serviceResponse.Code;

        }
        catch (Exception ex)
        {
            serviceResponse.ShortDescription = ex.Message;
            serviceResponse.Code = ((int)HttpStatusCode.BadRequest).ToString();
            if (!ModelState.IsValid)
            {
                serviceResponse.ValidationErrors = ModelState.ToDictionary(
                    m => {
                        var tokens = m.Key.Split('.');
                        return tokens.Length > 0 ? tokens[tokens.Length - 1] : tokens[0];
                    },
                    m => m.Value.Errors.Select(e => e.Exception?.Message ?? e.ErrorMessage)
                );
            }
            Log.Error(ex.Message, ex);
        }

        Log.Information($"EXITS ({method}) method");

        return serviceResponse;
    }
}