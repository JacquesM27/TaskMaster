using Microsoft.EntityFrameworkCore;
using TaskMaster.Modules.Teaching.Entities.OpenForm;
using TaskMaster.Modules.Teaching.Repositories;

namespace TaskMaster.Modules.Teaching.DAL.Repositories;

internal sealed class OpenFormAnswerRepository(TeachingDbContext dbContext) : IOpenFormAnswerRepository
{
    public async Task AddEssayAnswerAsync(EssayAnswer answer, CancellationToken cancellationToken)
    {
        await dbContext.EssayAnswers.AddAsync(answer, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<EssayAnswer?> GetEssayAnswerAsync(Guid id, CancellationToken cancellationToken)
        => dbContext.EssayAnswers.SingleOrDefaultAsync(ea => ea.Id == id, cancellationToken);

    public async Task AddMailAnswerAsync(MailAnswer answer, CancellationToken cancellationToken)
    {
        await dbContext.MailAnswers.AddAsync(answer, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<MailAnswer?> GetMailAnswerAsync(Guid id, CancellationToken cancellationToken)
        => dbContext.MailAnswers.SingleOrDefaultAsync(ma => ma.Id == id, cancellationToken);

    public async Task AddSummaryOfTextAnswerAsync(SummaryOfTextAnswer answer, CancellationToken cancellationToken)
    {
        await dbContext.SummaryOfTextAnswers.AddAsync(answer, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<SummaryOfTextAnswer?> GetSummaryOfTextAnswerAsync(Guid id, CancellationToken cancellationToken)
        => dbContext.SummaryOfTextAnswers.SingleOrDefaultAsync(sta => sta.Id == id, cancellationToken);
}