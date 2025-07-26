using TaskMaster.Abstractions.Events;
using TaskMaster.Abstractions.Queries;
using TaskMaster.Abstractions.Serialization;
using TaskMaster.Events.Exercises.OpenForm;
using TaskMaster.Events.SupiciousPrompts;
using TaskMaster.Models.Exercises.OpenForm;
using TaskMaster.Models.Exercises.OpenForm.Queries;
using TaskMaster.OpenAi.Services;

namespace TaskMaster.Modules.Exercises.QueryHandlers;

internal sealed class MailQueryHandler(
    IPromptFormatter promptFormatter,
    IObjectSamplerService objectSamplerService,
    IOpenAiExerciseService openAiExerciseService,
    IEventDispatcher eventDispatcher,
    ICustomSerializer customSerializer) : IQueryHandler<MailQuery, MailResponseOpenForm>
{
    public async Task<MailResponseOpenForm> HandleAsync(MailQuery query)
    {
        var queryAsString = objectSamplerService.GetStringValues(query);
        var suspiciousPromptResponse = await openAiExerciseService.ValidateAvoidingOriginTopic(queryAsString);
        if (suspiciousPromptResponse.IsSuspicious)
        {
            await eventDispatcher.PublishAsync(new SuspiciousPromptInjected(suspiciousPromptResponse.Reasons));
            throw new PromptInjectionException(suspiciousPromptResponse.Reasons);
        }
        
        var exerciseJsonFormat = objectSamplerService.GetSampleJson(typeof(Mail));
        
        var prompt =
            "1. This is open form - mail exercise. This means that you need to generate a short description of the email to be written by the student. Add information on who the email should be to.";
        prompt += promptFormatter.FormatExerciseBaseData(query);
        prompt +=
            $"12. In instruction field include information about the minimum number of words in email - {query.MinimumNumberOfWords}. It's important.\n";
        prompt += $"""
                   13. Your responses should be structured in JSON format as follows:
                   {exerciseJsonFormat}
                   """;

        var response =
            await openAiExerciseService.CompleteChatAsync(prompt, query.MotherLanguage, query.TargetLanguage);

        var exercise = customSerializer.TryDeserialize<Mail>(response)
                       ?? throw new DeserializationException(response);

        var result = new MailResponseOpenForm()
        {
            Exercise = exercise,
            ExerciseHeaderInMotherLanguage = query.ExerciseHeaderInMotherLanguage,
            MotherLanguage = query.MotherLanguage,
            TargetLanguage = query.TargetLanguage,
            TargetLanguageLevel = query.TargetLanguageLevel,
            TopicsOfSentences = query.TopicsOfSentences,
            GrammarSection = query.GrammarSection
        };

        await eventDispatcher.PublishAsync(new OpenFormGenerated<Mail>(result.Id, exercise,
            result.ExerciseHeaderInMotherLanguage, result.MotherLanguage, result.TargetLanguage,
            result.TargetLanguageLevel, result.TopicsOfSentences, result.GrammarSection));
        return result;
    }
}