using TaskMaster.Infrastructure.Exceptions;

namespace TaskMaster.OpenAi.Exceptions;

public sealed class PromptInjectionException(IEnumerable<string> reasons)
    : CustomException($"Prompt contains an invalid injection value. Reasons: {string.Join(", ", reasons)}");