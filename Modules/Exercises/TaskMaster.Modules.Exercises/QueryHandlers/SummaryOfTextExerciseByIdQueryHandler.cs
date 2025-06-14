using TaskMaster.Abstractions.Queries;
using TaskMaster.Models.Exercises.OpenForm;
using TaskMaster.Modules.Exercises.Services;
using TasMaster.Queries.Exercises.OpenForm;

namespace TaskMaster.Modules.Exercises.QueryHandlers;

internal sealed class SummaryOfTextExerciseByIdQueryHandler(
    IOpenFormService service)
    : IQueryHandler<SummaryOfTextExerciseByIdQuery, SummaryOfTextDto?>
{
    public Task<SummaryOfTextDto?> HandleAsync(SummaryOfTextExerciseByIdQuery query)
    {
        return service.GetSummaryOfTextAsync(query.ExerciseId, query.CancellationToken);
    }
}