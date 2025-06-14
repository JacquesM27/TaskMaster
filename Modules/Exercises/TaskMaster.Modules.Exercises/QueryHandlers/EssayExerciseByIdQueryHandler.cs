using TaskMaster.Abstractions.Queries;
using TaskMaster.Models.Exercises.OpenForm;
using TaskMaster.Modules.Exercises.Services;
using TasMaster.Queries.Exercises.OpenForm;

namespace TaskMaster.Modules.Exercises.QueryHandlers;

internal sealed class EssayExerciseByIdQueryHandler(
    IOpenFormService service)
    : IQueryHandler<EssayExerciseByIdQuery, EssayDto?>
{
    public Task<EssayDto?> HandleAsync(EssayExerciseByIdQuery query)
    {
        return service.GetEssayAsync(query.ExerciseId, query.CancellationToken);
    }
}