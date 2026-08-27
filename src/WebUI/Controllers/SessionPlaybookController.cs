#pragma warning disable
#pragma info disable
using System.Collections.Generic;
using System.Threading.Tasks;
using ArrayApp.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ArrayApp.WebUI.Controllers;

// =========================================================================================================
// [NEW CORE ARCHITECTURAL ADDITION]: SessionPlaybookController
// REST API exposing workshop playbooks (SCAMPER, Six Hats, Crazy 8s, Investor Pitch) and facilitation agendas
// =========================================================================================================
[ApiController]
[Route("api/[controller]")]
public class SessionPlaybookController : ControllerBase
{
    private readonly ISessionPlaybookService _playbookService;

    public SessionPlaybookController(ISessionPlaybookService playbookService)
    {
        _playbookService = playbookService;
    }

    [HttpGet]
    public async Task<ActionResult<List<WorkshopPlaybookDto>>> GetAllPlaybooks()
    {
        var playbooks = await _playbookService.GetAllPlaybooksAsync();
        return Ok(playbooks);
    }

    [HttpGet("{formatType}")]
    public async Task<ActionResult<WorkshopPlaybookDto>> GetPlaybookTemplate(string formatType)
    {
        var playbook = await _playbookService.GetPlaybookTemplateAsync(formatType);
        return Ok(playbook);
    }
}
