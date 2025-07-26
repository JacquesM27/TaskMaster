using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Chat;
using TaskMaster.Infrastructure.Settings;

namespace TaskMaster.OpenAi.Services;

internal sealed class OpenAiExerciseService(IOptions<OpenAiSettings> settings) : IOpenAiExerciseService
{
    private readonly OpenAIClient _client = new(settings.Value.ApiKey);
    private const string DefaultModel = "gpt-4o-2024-11-20";

    public async Task<string> CompleteChatAsync(string startMessage, string prompt)
    {
        var messages = new ChatMessage[]
        {
            ChatMessage.CreateSystemMessage(startMessage),
            ChatMessage.CreateUserMessage(prompt)
        };
        var chatResponse = await _client.GetChatClient(DefaultModel).CompleteChatAsync(messages);
        var response = chatResponse.Value.Content[0].Text;
        return response;
    }
}