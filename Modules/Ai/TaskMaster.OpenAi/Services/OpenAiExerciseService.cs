using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Chat;
using TaskMaster.Infrastructure.Settings;
using TaskMaster.Models.Exercises.Base;

namespace TaskMaster.OpenAi.Services;

internal sealed class OpenAiExerciseService : IOpenAiExerciseService
{
    private readonly IPromptFormatter _promptFormatter;
    private OpenAIClient _client;
    private readonly IObjectSamplerService _objectSamplerService;

    public OpenAiExerciseService(IPromptFormatter promptFormatter,
        IOptions<OpenAiSettings> settings, IObjectSamplerService objectSamplerService)
    {
        _promptFormatter = promptFormatter;
        _objectSamplerService = objectSamplerService;
        _client = new OpenAIClient(settings.Value.ApiKey);
    }

    public async Task<string> PromptForExercise(string prompt, string motherLanguage, string targetLanguage)
    {
        var startMessage = _promptFormatter.FormatStartingSystemMessage(motherLanguage, targetLanguage);
        
        // var schema = _objectSamplerService.GetStaticJsonSchema(typeof(TExercise));
        // var options = new ChatCompletionOptions
        // {
        //     ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
        //         jsonSchemaFormatName: $"{typeof(TExercise).Name}_schema",
        //         jsonSchema: BinaryData.FromBytes(Encoding.UTF8.GetBytes(schema)),
        //         jsonSchemaIsStrict: true)
        // };

        var messages = new ChatMessage[]
        {
            ChatMessage.CreateSystemMessage(startMessage),
            ChatMessage.CreateUserMessage(prompt)
        };
        var chatResponse = await _client.GetChatClient("gpt-4o-2024-11-20").CompleteChatAsync(messages);
        var response = chatResponse.Value.Content[0].Text;
        return response;
    }

    public async Task<SuspiciousPrompt> ValidateAvoidingOriginTopic(string prompt)
    {
        var startMessage = _promptFormatter.FormatValidationSystemMessage();

        var messages = new ChatMessage[]
        {
            ChatMessage.CreateSystemMessage(startMessage),
            ChatMessage.CreateUserMessage(prompt)
        };

        // var schema = _objectSamplerService.GetSampleJson(typeof(SuspiciousPrompt));
        // var options = new ChatCompletionOptions();
        // options.ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
        //     jsonSchemaFormatName: $"{nameof(SuspiciousPrompt)}_schema",
        //     jsonSchema: BinaryData.FromBytes(Encoding.UTF8.GetBytes(schema)),
        //     jsonSchemaIsStrict: true);

        var chatResponse = await _client.GetChatClient("gpt-4o-2024-11-20").CompleteChatAsync(messages);
        var response = chatResponse.Value.Content[0].Text;
        var result = JsonSerializer.Deserialize<SuspiciousPrompt>(response);
        return result;
    }
}