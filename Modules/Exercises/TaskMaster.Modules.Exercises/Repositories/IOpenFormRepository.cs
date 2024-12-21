using TaskMaster.Modules.Exercises.Entities;

namespace TaskMaster.Modules.Exercises.Repositories;

public interface IOpenFormRepository
{
    Task<Mail?> GetMailAsync(Guid id, CancellationToken cancellationToken);
    Task<Essay?> GetEssayAsync(Guid id, CancellationToken cancellationToken);
    Task<SummaryOfText?> GetSummaryOfTextAsync(Guid id, CancellationToken cancellationToken);
    
    Task AddMailAsync(Mail exercise, CancellationToken cancellationToken);

    Task AddEssayAsync(Essay exercise, CancellationToken cancellationToken);

    Task AddSummaryOfTextAsync(SummaryOfText exercise, CancellationToken cancellationToken);
}