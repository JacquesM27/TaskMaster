using TaskMaster.OpenAi.Models;

namespace TaskMaster.OpenAi.Services;

internal interface IOpenAiExerciseService
{
    Task<string> PromptForExercise(string prompt, string motherLanguage, string targetLanguage);

    Task<SuspiciousPrompt> ValidateAvoidingOriginTopic(string prompt);
}