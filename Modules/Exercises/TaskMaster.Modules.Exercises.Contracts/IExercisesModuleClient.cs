using TaskMaster.Abstractions.Modules;
using TaskMaster.Models.Exercises.OpenForm;

namespace TaskMaster.Modules.Exercises.Contracts;

public interface IExercisesModuleClient : IModuleClient
{
    Task<MailDto?> GetMailExerciseAsync(Guid exerciseId, CancellationToken cancellationToken = default);
    Task<EssayDto?> GetEssayExerciseAsync(Guid exerciseId, CancellationToken cancellationToken = default);
    Task<SummaryOfTextDto?> GetSummaryOfTextExerciseAsync(Guid exerciseId, CancellationToken cancellationToken = default);
    Task<bool> IsExerciseVerifiedAsync(Guid exerciseId, CancellationToken cancellationToken = default);
    Task<IEnumerable<MailDto>> GetMailExercisesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<EssayDto>> GetEssayExercisesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<SummaryOfTextDto>> GetSummaryOfTextExercisesAsync(CancellationToken cancellationToken = default);
}