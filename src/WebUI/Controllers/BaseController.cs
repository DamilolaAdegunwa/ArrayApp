using System.Net;
using System.Runtime.CompilerServices;
using ArrayApp.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ArrayApp.WebUI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BaseController : ApiControllerBase
{
    private ILogger? _logger;
    protected ILogger Logger => _logger ??= HttpContext?.RequestServices.GetService<ILogger<BaseController>>() ?? new LoggerFactory().CreateLogger<BaseController>();

    public BaseController(ILogger? logger = null)
    {
        _logger = logger;
    }

    protected async Task<ServiceResponse<T>> HandleApiOperationAsync<T>(
        Func<Task<ServiceResponse<T>>> action, [CallerLineNumber] int lineNo = 0, [CallerMemberName] string method = "")
    {
        Logger.LogInformation("ENTERS ({Method}) method", method);

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
            Logger.LogError(ex, "{ErrorMessage}", ex.Message);
        }

        Logger.LogInformation("EXITS ({Method}) method", method);

        return serviceResponse;
    }
}