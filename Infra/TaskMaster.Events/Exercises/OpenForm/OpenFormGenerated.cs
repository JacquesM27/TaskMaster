using TaskMaster.Abstractions.Events;

namespace TaskMaster.Events.Exercises.OpenForm;

public sealed record OpenFormGenerated<TExercise>(
    Guid Id,
    TExercise Exercise,
    bool ExerciseHeaderInMotherLanguage,
    string MotherLanguage,
    string TargetLanguage,
    string TargetLanguageLevel,
    string? TopicsOfSentences,
    string? GrammarSection) : IEvent where TExercise : TaskMaster.Models.Exercises.OpenForm.OpenForm;

// public sealed class FakeHandler() : IEventHandler<OpenFormGenerated, >
// {
//     public Task HandleAsync(OpenFormGenerated @event)
//     {
//         return Task.CompletedTask;
//     }
// }