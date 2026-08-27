#pragma warning disable
#pragma info disable
using System.Threading.Tasks;
using ArrayApp.Application.Ideas.Commands;
using ArrayApp.Application.Ideas.Queries;
using Microsoft.AspNetCore.Mvc;

namespace ArrayApp.WebUI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProvenanceController : ControllerBase
{
    private readonly ISender _mediator;

    public ProvenanceController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("certificate/{ideaId}")]
    public async Task<ActionResult<W3CVerifiableCertificateDto>> GenerateCertificate(int ideaId, [FromQuery] string? issuerDid)
    {
        var command = new GenerateProvenanceCertificateCommand(ideaId, issuerDid ?? "did:arrayapp:org:governance");
        var cert = await _mediator.Send(command);
        return Ok(cert);
    }

    [HttpGet("verify/{ideaId}")]
    public async Task<ActionResult<ProvenanceChainVerificationDto>> VerifyChain(int ideaId)
    {
        var result = await _mediator.Send(new VerifyProvenanceChainQuery(ideaId));
        return Ok(result);
    }
}
