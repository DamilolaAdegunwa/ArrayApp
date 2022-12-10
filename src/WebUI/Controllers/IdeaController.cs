using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace ArrayApp.WebUI.Controllers;
[Authorize]
public class IdeaController : ApiControllerBase
{
    [HttpGet]
    public IActionResult Index()
    {
        return Ok();
    }
}
