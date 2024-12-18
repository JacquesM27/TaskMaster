namespace TaskMaster.Models.Exercises.OpenForm;

public class OpenFormDto<TExercise> where TExercise : OpenForm
{
    public Guid Id { get; set; }
    public TExercise Exercise { get; set; }
    public bool ExerciseHeaderInMotherLanguage { get; set; }
    public string MotherLanguage { get; set; }
    public string TargetLanguage { get; set; }
    public string TargetLanguageLevel { get; set; }
    public string? TopicsOfSentences { get; set; }
    public string? GrammarSection { get; set; }
    public bool VerifiedByTeacher { get; set; }
}

public class MailDto : OpenFormDto<Mail>;

public class EssayDto : OpenFormDto<Essay>;

public class SummaryOfTextDto : OpenFormDto<SummaryOfText>;