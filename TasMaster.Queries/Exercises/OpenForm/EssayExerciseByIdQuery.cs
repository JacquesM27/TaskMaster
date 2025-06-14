using TaskMaster.Abstractions.Queries;
using TaskMaster.Models.Exercises.OpenForm;

namespace TasMaster.Queries.Exercises.OpenForm;

public sealed record EssayExerciseByIdQuery(Guid ExerciseId, CancellationToken CancellationToken) : IQuery<EssayDto?>;