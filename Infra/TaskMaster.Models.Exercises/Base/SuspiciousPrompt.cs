namespace TaskMaster.Models.Exercises.Base;

public sealed class SuspiciousPrompt
{
    public bool IsSuspicious { get; set; } = false;
    public List<string> Reasons { get; set; } = [];
}