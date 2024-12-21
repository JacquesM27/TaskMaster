using TaskMaster.Models.Exercises.OpenForm;

namespace TaskMaster.Modules.Exercises.Services;

public interface IOpenFormService
{
    Task<MailDto?> GetMailAsync(Guid id, CancellationToken cancellationToken);
    Task<EssayDto?> GetEssayAsync(Guid id, CancellationToken cancellationToken);
    Task<SummaryOfTextDto?> GetSummaryOfTextAsync(Guid id, CancellationToken cancellationToken);
    Task<Guid> AddMailAsync(MailDto dto, CancellationToken cancellationToken);
    Task<Guid> AddEssayAsync(EssayDto dto, CancellationToken cancellationToken);
    Task<Guid> AddSummaryOfTextAsync(SummaryOfTextDto dto, CancellationToken cancellationToken);
}