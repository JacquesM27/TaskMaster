using TaskMaster.Models.Exercises.OpenForm;
using TaskMaster.Models.Exercises.Requests.OpenForm;

namespace TaskMaster.Modules.Exercises.Services;

public interface IOpenFormGenerationService
{
    public Task<EssayResponseOpenForm> GenerateEssay(EssayRequestDto request);
    public Task<MailResponseOpenForm> GenerateMail(MailRequestDto request);
    public Task<SummaryOfTextResponseOpenForm> GenerateSummaryOfText(SummaryOfTextRequestDto request);
}