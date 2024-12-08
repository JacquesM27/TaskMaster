using OpenAI.Chat;

namespace TaskMaster.OpenAi.Extensions;

internal static class ChatFormatExtensions
{
    internal static ChatCompletionOptions FormatChatCompletionOptions(string jsonSchema, string exerciseName)
    {
        var options = new ChatCompletionOptions
        {
            ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                jsonSchemaIsStrict: true,
                jsonSchemaFormatName: $"exercise-{exerciseName}",
                jsonSchema: BinaryData.FromBytes(System.Text.Encoding.UTF8.GetBytes(jsonSchema)))
        };
        return options;
    }
}