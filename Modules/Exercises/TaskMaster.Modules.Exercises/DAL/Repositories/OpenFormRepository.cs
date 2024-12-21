using Microsoft.EntityFrameworkCore;
using TaskMaster.Modules.Exercises.Entities;
using TaskMaster.Modules.Exercises.Repositories;

namespace TaskMaster.Modules.Exercises.DAL.Repositories;

internal sealed class OpenFormRepository(ExercisesDbContext context) : IOpenFormRepository
{
    public Task<Mail?> GetMailAsync(Guid id, CancellationToken cancellationToken) 
        => context.Mails.SingleOrDefaultAsync(m => m.Id == id, cancellationToken);

    public Task<Essay?> GetEssayAsync(Guid id, CancellationToken cancellationToken) 
        => context.Essays.SingleOrDefaultAsync(m => m.Id == id, cancellationToken);

    public Task<SummaryOfText?> GetSummaryOfTextAsync(Guid id, CancellationToken cancellationToken) 
        => context.SummariesOfText.SingleOrDefaultAsync(m => m.Id == id, cancellationToken);

    public async Task AddMailAsync(Mail exercise, CancellationToken cancellationToken)
    {
        await context.Mails.AddAsync(exercise, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task AddEssayAsync(Essay exercise, CancellationToken cancellationToken)
    {
        await context.Essays.AddAsync(exercise, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task AddSummaryOfTextAsync(SummaryOfText exercise, CancellationToken cancellationToken)
    {
        await context.SummariesOfText.AddAsync(exercise, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}