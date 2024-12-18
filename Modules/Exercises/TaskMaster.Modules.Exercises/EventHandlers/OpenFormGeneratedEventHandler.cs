using TaskMaster.Abstractions.Events;
using TaskMaster.Events.Exercises.OpenForm;
using TaskMaster.Models.Exercises.OpenForm;
using TaskMaster.Modules.Exercises.Repositories;

namespace TaskMaster.Modules.Exercises.EventHandlers;

public class OpenFormGeneratedEventHandler<TExercise>(IOpenFormRepository repository)
    : IEventHandler<OpenFormGenerated<TExercise>> where TExercise : OpenForm
{
    public Task HandleAsync(OpenFormGenerated<TExercise> @event)
    {
        return @event.Exercise switch
        {
            Mail mail => repository.AddMailAsync(mail, @event.Id, @event.ExerciseHeaderInMotherLanguage,
                @event.MotherLanguage, @event.TargetLanguage, @event.TargetLanguageLevel, @event.TopicsOfSentences,
                @event.GrammarSection),
            Essay essay => repository.AddEssayAsync(essay, @event.Id, @event.ExerciseHeaderInMotherLanguage,
                @event.MotherLanguage, @event.TargetLanguage, @event.TargetLanguageLevel, @event.TopicsOfSentences,
                @event.GrammarSection),
            SummaryOfText summaryOfText => repository.AddSummaryOfTextAsync(summaryOfText, @event.Id,
                @event.ExerciseHeaderInMotherLanguage, @event.MotherLanguage, @event.TargetLanguage,
                @event.TargetLanguageLevel, @event.TopicsOfSentences, @event.GrammarSection),
            _ => Task.CompletedTask
        };
    }
}