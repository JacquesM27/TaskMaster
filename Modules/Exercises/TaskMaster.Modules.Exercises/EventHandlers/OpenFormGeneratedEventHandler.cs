using TaskMaster.Abstractions.Events;
using TaskMaster.Events.Exercises.OpenForm;
using TaskMaster.Models.Exercises.OpenForm;
using TaskMaster.Modules.Exercises.Services;

namespace TaskMaster.Modules.Exercises.EventHandlers;

public class OpenFormGeneratedEventHandler<TExercise>(IOpenFormService service)
    : IEventHandler<OpenFormGenerated<TExercise>> where TExercise : OpenForm
{
    public Task HandleAsync(OpenFormGenerated<TExercise> @event)
    {
        return @event.Exercise switch
        {
            Mail mail =>
                service.AddMailAsync(new MailDto
                {
                    Id = @event.Id,
                    Exercise = mail,
                    GrammarSection = @event.GrammarSection,
                    MotherLanguage = @event.MotherLanguage,
                    TargetLanguage = @event.TargetLanguage,
                    TargetLanguageLevel = @event.TargetLanguageLevel,
                    TopicsOfSentences = @event.TopicsOfSentences,
                    ExerciseHeaderInMotherLanguage = @event.ExerciseHeaderInMotherLanguage,
                }, CancellationToken.None),
            Essay essay =>
                service.AddEssayAsync(new EssayDto
                {
                    Id = @event.Id,
                    Exercise = essay,
                    GrammarSection = @event.GrammarSection,
                    MotherLanguage = @event.MotherLanguage,
                    TargetLanguage = @event.TargetLanguage,
                    TargetLanguageLevel = @event.TargetLanguageLevel,
                    TopicsOfSentences = @event.TopicsOfSentences,
                    ExerciseHeaderInMotherLanguage = @event.ExerciseHeaderInMotherLanguage,
                }, CancellationToken.None),
            SummaryOfText summaryOfText =>
                service.AddSummaryOfTextAsync(new SummaryOfTextDto
                {
                    Id = @event.Id,
                    Exercise = summaryOfText,
                    GrammarSection = @event.GrammarSection,
                    MotherLanguage = @event.MotherLanguage,
                    TargetLanguage = @event.TargetLanguage,
                    TargetLanguageLevel = @event.TargetLanguageLevel,
                    TopicsOfSentences = @event.TopicsOfSentences,
                    ExerciseHeaderInMotherLanguage = @event.ExerciseHeaderInMotherLanguage,
                }, CancellationToken.None),
            _ => Task.CompletedTask
        };
    }
}