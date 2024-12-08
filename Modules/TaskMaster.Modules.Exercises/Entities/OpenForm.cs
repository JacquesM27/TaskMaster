namespace TaskMaster.Modules.Exercises.Entities;

internal sealed class OpenForm
{
    public Guid Id { get; set; }
    public string Exercise { get; set; } = string.Empty;
    public bool ExerciseHeaderInMotherLanguage { get; set; }
    public string MotherLanguage { get; set; }
    public string TargetLanguage { get; set; }
    public string TargetLanguageLevel { get; set; }
    public string? TopicsOfSentences { get; set; }
    public string? GrammarSection { get; set; }
    public bool VerifiedByTeacher { get; set; }
}