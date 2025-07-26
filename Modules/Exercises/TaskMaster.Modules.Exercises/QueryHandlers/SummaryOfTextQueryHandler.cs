using TaskMaster.Abstractions.Events;
using TaskMaster.Abstractions.Queries;
using TaskMaster.Abstractions.Serialization;
using TaskMaster.Events.Exercises.OpenForm;
using TaskMaster.Events.SupiciousPrompts;
using TaskMaster.Models.Exercises.OpenForm;
using TaskMaster.Models.Exercises.OpenForm.Queries;
using TaskMaster.OpenAi.Services;

namespace TaskMaster.Modules.Exercises.QueryHandlers;

internal sealed class SummaryOfTextQueryHandler(
    IPromptFormatter promptFormatter,
    IObjectSamplerService objectSamplerService,
    IOpenAiExerciseService openAiExerciseService,
    IEventDispatcher eventDispatcher,
    ICustomSerializer customSerializer) : IQueryHandler<SummaryOfTextQuery, SummaryOfTextResponseOpenForm>
{
    public async Task<SummaryOfTextResponseOpenForm> HandleAsync(SummaryOfTextQuery query)
    {
         var queryAsString = objectSamplerService.GetStringValues(query);
         var suspiciousPromptResponse = await openAiExerciseService.ValidateAvoidingOriginTopic(queryAsString);
         if (suspiciousPromptResponse.IsSuspicious)
         {
             await eventDispatcher.PublishAsync(new SuspiciousPromptInjected(suspiciousPromptResponse.Reasons));
             throw new PromptInjectionException(suspiciousPromptResponse.Reasons);
         }
         var exerciseJsonFormat = objectSamplerService.GetSampleJson(typeof(SummaryOfText));

         var prompt =
             "1. This is open form - summary of text exercise. This means that you need to generate a story (about 10 sentences) to be summarized by the student.";
         prompt += promptFormatter.FormatExerciseBaseData(query);
         prompt += $"""
                    12. Your responses should be structured in JSON format as follows:
                    {exerciseJsonFormat}
                    """;

         var response =
             await openAiExerciseService.CompleteChatAsync(prompt, query.MotherLanguage, query.TargetLanguage);

         var exercise = customSerializer.TryDeserialize<SummaryOfText>(response)
                        ?? throw new DeserializationException(response);

         var result = new SummaryOfTextResponseOpenForm()
         {
             Exercise = exercise,
             ExerciseHeaderInMotherLanguage = query.ExerciseHeaderInMotherLanguage,
             MotherLanguage = query.MotherLanguage,
             TargetLanguage = query.TargetLanguage,
             TargetLanguageLevel = query.TargetLanguageLevel,
             TopicsOfSentences = query.TopicsOfSentences,
             GrammarSection = query.GrammarSection
         };

         await eventDispatcher.PublishAsync(new OpenFormGenerated<SummaryOfText>(result.Id, exercise,
             result.ExerciseHeaderInMotherLanguage, result.MotherLanguage, result.TargetLanguage,
             result.TargetLanguageLevel, result.TopicsOfSentences, result.GrammarSection));
         return result;
    }
}