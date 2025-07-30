using TaskMaster.Models.Exercises.OpenForm;
using TaskMaster.Models.Exercises.Requests.OpenForm;

namespace TaskMaster.Modules.Exercises.Services;

public interface IOpenFormGenerationService
{
    public Task<EssayDto> GenerateEssay(EssayRequestDto request, CancellationToken cancellationToken);
    public Task<MailDto> GenerateMail(MailRequestDto request, CancellationToken cancellationToken);
    public Task<SummaryOfTextDto> GenerateSummaryOfText(SummaryOfTextRequestDto request, CancellationToken cancellationToken);
}