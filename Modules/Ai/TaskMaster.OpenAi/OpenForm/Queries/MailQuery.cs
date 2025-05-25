using System.Text.Json;
using TaskMaster.Abstractions.Events;
using TaskMaster.Abstractions.Queries;
using TaskMaster.Events.Exercises.OpenForm;
using TaskMaster.Events.SupiciousPrompts;
using TaskMaster.Models.Exercises.Base;
using TaskMaster.Models.Exercises.OpenForm;
using TaskMaster.OpenAi.Exceptions;
using TaskMaster.OpenAi.Services;

namespace TaskMaster.OpenAi.OpenForm.Queries;

internal sealed class MailQuery : ExerciseQueryBase, IQuery<MailResponseOpenForm>
{
    public int MinimumNumberOfWords { get; set; }
}

internal sealed class MailQueryHandler(
    IPromptFormatter promptFormatter,
    IObjectSamplerService objectSamplerService,
    IOpenAiExerciseService openAiExerciseService,
    IEventDispatcher eventDispatcher
    ) : IQueryHandler<MailQuery, MailResponseOpenForm>
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
            await openAiExerciseService.PromptForExercise(prompt, query.MotherLanguage, query.TargetLanguage);

        var exercise = JsonSerializer.Deserialize<Mail>(response)
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