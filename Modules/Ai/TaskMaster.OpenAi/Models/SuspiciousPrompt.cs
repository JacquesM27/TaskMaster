namespace TaskMaster.OpenAi.Models;

internal sealed class SuspiciousPrompt
{
    public bool IsSuspicious { get; set; } = false;
    public List<string> Reasons { get; set; } = [];
}