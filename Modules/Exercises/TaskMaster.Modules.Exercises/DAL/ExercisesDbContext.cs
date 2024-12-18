using Microsoft.EntityFrameworkCore;
using TaskMaster.Modules.Exercises.Entities;

namespace TaskMaster.Modules.Exercises.DAL;

internal sealed class ExercisesDbContext(DbContextOptions<ExercisesDbContext> options)
    : DbContext(options)
{
    public DbSet<Mail> Mails { get; set; }
    public DbSet<SummaryOfText> SummariesOfText { get; set; }
    public DbSet<Essay> Essays { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Exercises");
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}