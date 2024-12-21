namespace TaskMaster.Modules.Exercises.Entities;

public sealed class SummaryOfText
{
    public Guid Id { get; set; }
    public Models.Exercises.OpenForm.SummaryOfText Exercise { get; set; }
    public bool ExerciseHeaderInMotherLanguage { get; set; }
    public string MotherLanguage { get; set; }
    public string TargetLanguage { get; set; }
    public string TargetLanguageLevel { get; set; }
    public string? TopicsOfSentences { get; set; }
    public string? GrammarSection { get; set; }
    public bool VerifiedByTeacher { get; set; }
}