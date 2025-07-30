namespace TaskMaster.OpenAi.Services;

public interface IOpenAiExerciseService
{
    Task<string> CompleteChatAsync(string startMessage, string prompt, CancellationToken cancellationToken);
}