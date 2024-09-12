namespace TaskMaster.OpenAi.Models;

internal abstract class ExerciseResponse<TExercise> where TExercise : Exercise
{
    public Guid Id { get; } = Guid.NewGuid();
    public TExercise Exercise { get; set; }
    public bool ExerciseHeaderInMotherLanguage { get; set; }
    public string MotherLanguage { get; set; }
    public string TargetLanguage { get; set; }
    public string TargetLanguageLevel { get; set; }
    public string? TopicsOfSentences { get; set; }
    public string? GrammarSection { get; set; }
}