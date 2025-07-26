namespace TaskMaster.Models.Exercises.Requests.OpenForm;

public sealed record MailRequestDto : ExerciseRequestBase
{
    public int MinimumNumberOfWords { get; init; }
}