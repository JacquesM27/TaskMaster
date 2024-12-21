using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskMaster.Models.Exercises.OpenForm;
using TaskMaster.Modules.Exercises.Services;

namespace TaskMaster.Modules.Exercises.Controllers;

[ApiController]
[Route("open-form")]
[Tags("OpenForm")]
[Authorize]
public class OpenFormController(IOpenFormService service) : ControllerBase
{
    [HttpGet("ping")]
    public ActionResult<string> Ping()
    {
        return Ok("Pong");
    }

    [HttpPost("mail")]
    public async Task<ActionResult<Guid>> AddMail(MailDto dto, CancellationToken cancellationToken)
    {
        return Ok(await service.AddMailAsync(dto, cancellationToken));
    }
    
    [HttpGet("mail/{id:guid}")]
    public async Task<ActionResult<MailDto?>> GetMail(Guid id, CancellationToken cancellationToken)
    {
        var mail = await service.GetMailAsync(id, cancellationToken);
        if (mail is null)
            return NotFound();
        return Ok(mail);
    }
    
    [HttpPost("essay")]
    public async Task<ActionResult<Guid>> AddEssay(EssayDto dto, CancellationToken cancellationToken)
    {
        return Ok(await service.AddEssayAsync(dto, cancellationToken));
    }
    
    [HttpGet("essay/{id:guid}")]
    public async Task<ActionResult<EssayDto?>> GetEssay(Guid id, CancellationToken cancellationToken)
    {
        var essay = await service.GetEssayAsync(id, cancellationToken);
        if (essay is null)
            return NotFound();
        return Ok(essay);
    }
    
    [HttpPost("summary-of-text")]
    public async Task<ActionResult<Guid>> AddSummaryOfText(SummaryOfTextDto dto, CancellationToken cancellationToken)
    {
        return Ok(await service.AddSummaryOfTextAsync(dto, cancellationToken));
    }
    
    [HttpGet("summary-of-text/{id:guid}")]
    public async Task<ActionResult<SummaryOfTextDto?>> GetSummaryOfText(Guid id, CancellationToken cancellationToken)
    {
        var summaryOfText = await service.GetSummaryOfTextAsync(id, cancellationToken);
        if (summaryOfText is null)
            return NotFound();
        return Ok(summaryOfText);
    }
}