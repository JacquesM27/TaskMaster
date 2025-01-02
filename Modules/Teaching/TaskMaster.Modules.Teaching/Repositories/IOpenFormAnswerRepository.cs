using TaskMaster.Modules.Teaching.Entities.OpenForm;

namespace TaskMaster.Modules.Teaching.Repositories;

public interface IOpenFormAnswerRepository
{
    Task AddEssayAnswerAsync(EssayAnswer answer, CancellationToken cancellationToken);
    Task<EssayAnswer?> GetEssayAnswerAsync(Guid id, CancellationToken cancellationToken);
    
    Task AddMailAnswerAsync(MailAnswer answer, CancellationToken cancellationToken);
    Task<MailAnswer?> GetMailAnswerAsync(Guid id, CancellationToken cancellationToken);
    
    Task AddSummaryOfTextAnswerAsync(SummaryOfTextAnswer answer, CancellationToken cancellationToken);
    Task<SummaryOfTextAnswer?> GetSummaryOfTextAnswerAsync(Guid id, CancellationToken cancellationToken);
}