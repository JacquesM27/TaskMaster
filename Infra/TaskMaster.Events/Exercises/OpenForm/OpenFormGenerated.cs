using TaskMaster.Abstractions.Events;

namespace TaskMaster.Events.Exercises.OpenForm;

public sealed record OpenFormGenerated(
    Guid Id,
    string Exercise,
    bool ExerciseHeaderInMotherLanguage,
    string MotherLanguage,
    string TargetLanguage,
    string TargetLanguageLevel,
    string? TopicsOfSentences,
    string? GrammarSection) : IEvent;

public sealed class FakeHandler() : IEventHandler<OpenFormGenerated>
{
    public Task HandleAsync(OpenFormGenerated @event)
    {
        return Task.CompletedTask;
    }
}