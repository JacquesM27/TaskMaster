using TaskMaster.Abstractions.Queries;
using TaskMaster.Models.Exercises.OpenForm;

namespace TasMaster.Queries.Exercises.OpenForm;

public sealed record MailExerciseByIdQuery(Guid ExerciseId, CancellationToken CancellationToken) : IQuery<MailDto?>;