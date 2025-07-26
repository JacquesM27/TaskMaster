namespace TaskMaster.Models.Exercises.Requests.OpenForm;

public sealed record EssayRequestDto : ExerciseRequestBase
{
    public int MinimumNumberOfWords { get; init; }
}