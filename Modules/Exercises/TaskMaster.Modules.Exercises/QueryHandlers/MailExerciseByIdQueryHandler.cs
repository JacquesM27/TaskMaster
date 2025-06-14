using TaskMaster.Abstractions.Queries;
using TaskMaster.Models.Exercises.OpenForm;
using TaskMaster.Modules.Exercises.Services;
using TasMaster.Queries.Exercises.OpenForm;

namespace TaskMaster.Modules.Exercises.QueryHandlers;

internal sealed class MailExerciseByIdQueryHandler(
    IOpenFormService service)
    : IQueryHandler<MailExerciseByIdQuery, MailDto?>
{
    public Task<MailDto?> HandleAsync(MailExerciseByIdQuery query)
    {
        return service.GetMailAsync(query.ExerciseId, query.CancellationToken);
    }
}