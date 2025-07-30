using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskMaster.Models.Exercises.OpenForm;
using TaskMaster.Models.Exercises.Requests.OpenForm;
using TaskMaster.Modules.Exercises.Services;

namespace TaskMaster.Modules.Exercises.Controllers;

[ApiController]
[Route("open-form-ai")]
[Tags("OpenFormAi")]
[Authorize]
public class OpenFormAiController(IOpenFormGenerationService openFormGenerationService) : ControllerBase
{
    [HttpPost("essay")]
    public async Task<ActionResult<EssayDto>> GenerateEssay(EssayRequestDto request, CancellationToken cancellationToken)
    {
        var result = await openFormGenerationService.GenerateEssay(request, cancellationToken);
        return Ok(result);
    }
    
    [HttpPost("mail")]
    public async Task<ActionResult<MailDto>> GenerateMail(MailRequestDto request, CancellationToken cancellationToken)
    {
        var result = await openFormGenerationService.GenerateMail(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost("summary-of-text")]
    public async Task<ActionResult<SummaryOfTextDto>> GenerateSummaryOfText(SummaryOfTextRequestDto request, CancellationToken cancellationToken)
    {
        var result = await openFormGenerationService.GenerateSummaryOfText(request, cancellationToken);
        return Ok(result);
    }
}