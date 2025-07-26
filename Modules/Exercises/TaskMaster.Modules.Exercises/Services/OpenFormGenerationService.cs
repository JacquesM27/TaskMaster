using TaskMaster.Models.Exercises.OpenForm;
using TaskMaster.Models.Exercises.Requests.OpenForm;
using TaskMaster.OpenAi.Client.Clients;

namespace TaskMaster.Modules.Exercises.Services;

public class OpenFormGenerationService(IOpenAiExerciseClient client) : IOpenFormGenerationService
{
    public async Task<EssayResponseOpenForm> GenerateEssay(EssayRequestDto request)
    {
        try
        {
            var essay = await client.PromptForEssay(request);
            
        }
    }

    public Task<MailResponseOpenForm> GenerateMail(MailRequestDto request)
    {
        throw new NotImplementedException();
    }

    public Task<SummaryOfTextResponseOpenForm> GenerateSummaryOfText(SummaryOfTextRequestDto request)
    {
        throw new NotImplementedException();
    }
}