using TaskMaster.Abstractions.Events;
using TaskMaster.Events.Exercises.OpenForm;
using TaskMaster.Models.Exercises.OpenForm;

namespace TaskMaster.Modules.Exercises.EventHandlers;

public class OpenFormGeneratedEventHandler<TExcercise> 
    : IEventHandler<OpenFormGenerated<TExcercise>> where TExcercise : OpenForm
{
    public Task HandleAsync(OpenFormGenerated<TExcercise> @event)
    {
        throw new NotImplementedException();
    }
}