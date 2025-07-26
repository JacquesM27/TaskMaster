namespace TaskMaster.Models.Exercises.Requests;

public record ExerciseRequestBase
{
    public bool ExerciseHeaderInMotherLanguage { get; init; }
    public string MotherLanguage { get; init; }
    public string TargetLanguage { get; init; }
    public string TargetLanguageLevel { get; init; }
    public string? TopicsOfSentences { get; init; }
    public string? GrammarSection { get; init; }
    public string? SupportMaterial { get; init; }
}