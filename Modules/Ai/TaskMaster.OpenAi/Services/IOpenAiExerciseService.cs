
using TaskMaster.Models.Exercises.Base;

namespace TaskMaster.OpenAi.Services;

internal interface IOpenAiExerciseService
{
    Task<string> PromptForExercise<TExercise>(string prompt, string motherLanguage, string targetLanguage);

    Task<SuspiciousPrompt> ValidateAvoidingOriginTopic(string prompt);
}