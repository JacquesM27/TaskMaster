using TaskMaster.Models.Exercises.OpenForm;
using TaskMaster.Models.Exercises.Requests.OpenForm;
using TaskMaster.OpenAi.Client.Clients;
using TaskMaster.OpenAi.Client.Exceptions;

namespace TaskMaster.Modules.Exercises.Services;

internal sealed class OpenFormGenerationService(IOpenAiExerciseClient client, IOpenFormService service) : IOpenFormGenerationService
{
    public async Task<EssayDto> GenerateEssay(EssayRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            var essay = await client.PromptForEssay(request, cancellationToken);
            var dto = new EssayDto
            {
                Exercise = essay,
                ExerciseHeaderInMotherLanguage = request.ExerciseHeaderInMotherLanguage,
                MotherLanguage = request.MotherLanguage,
                TargetLanguage = request.TargetLanguage,
                TargetLanguageLevel = request.TargetLanguageLevel,
                TopicsOfSentences = request.TopicsOfSentences,
                GrammarSection = request.GrammarSection,
                VerifiedByTeacher = false
            };
            var id = await service.AddEssayAsync(dto, cancellationToken);
            dto.Id = id;
            return dto;
        }
        catch (PromptInjectionException ex)
        {
            //TODO: logging
            //TODO: event for suspicious prompt
            throw;
        }
        catch (DeserializationException ex)
        {
            //TODO: policy?
            throw;
        }
    }

    public async Task<MailDto> GenerateMail(MailRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            var mail = await client.PromptForMail(request, cancellationToken);
            var dto = new MailDto
            {
                Exercise = mail,
                ExerciseHeaderInMotherLanguage = request.ExerciseHeaderInMotherLanguage,
                MotherLanguage = request.MotherLanguage,
                TargetLanguage = request.TargetLanguage,
                TargetLanguageLevel = request.TargetLanguageLevel,
                TopicsOfSentences = request.TopicsOfSentences,
                GrammarSection = request.GrammarSection,
                VerifiedByTeacher = false
            };
            var id = await service.AddMailAsync(dto, cancellationToken);
            dto.Id = id;
            return dto;
        }
        catch (PromptInjectionException ex)
        {
            //TODO: logging
            //TODO: event for suspicious prompt
            throw;
        }
        catch (DeserializationException ex)
        {
            //TODO: policy?
            throw;
        }
    }

    public async Task<SummaryOfTextDto> GenerateSummaryOfText(SummaryOfTextRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            var summaryOfText = await client.PromptForSummaryOfText(request, cancellationToken);
            var dto = new SummaryOfTextDto
            {
                Exercise = summaryOfText,
                ExerciseHeaderInMotherLanguage = request.ExerciseHeaderInMotherLanguage,
                MotherLanguage = request.MotherLanguage,
                TargetLanguage = request.TargetLanguage,
                TargetLanguageLevel = request.TargetLanguageLevel,
                TopicsOfSentences = request.TopicsOfSentences,
                GrammarSection = request.GrammarSection,
                VerifiedByTeacher = false
            };
            var id = await service.AddSummaryOfTextAsync(dto, cancellationToken);
            dto.Id = id;
            return dto;
        }
        catch (PromptInjectionException ex)
        {
            //TODO: logging
            //TODO: event for suspicious prompt
            throw;
        }
        catch (DeserializationException ex)
        {
            //TODO: policy?
            throw;
        }
    }
}