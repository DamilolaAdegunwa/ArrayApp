using System.Net;
using ArrayApp.Application.Common.Models;
using ArrayApp.Application.Ideas.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace ArrayApp.WebUI.Controllers;
//[Authorize]
public class IdeaController : ApiControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(Result), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(Result), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Create(CreateIdeaCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
        //if (!result.status)
        //{
        //    return BadRequest(new Result
        //    {
        //        Succeeded = false,
        //        Errors = new string[] { result.message },
        //        Data = result.response
        //    });
        //}
        //return Ok(new Result
        //{
        //    Succeeded = true,
        //    Errors = null,
        //    Data = result.response
        //});
    }
}