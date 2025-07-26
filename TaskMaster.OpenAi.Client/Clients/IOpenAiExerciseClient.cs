using TaskMaster.Models.Exercises.OpenForm;
using TaskMaster.Models.Exercises.Requests.OpenForm;

namespace TaskMaster.OpenAi.Client.Clients;

public interface IOpenAiExerciseClient
{
    public Task<Essay> PromptForEssay(EssayRequestDto request);
    public Task<Mail> PromptForMail(MailRequestDto request);
    public Task<SummaryOfText> PromptForSummaryOfText(SummaryOfTextRequestDto request);
}