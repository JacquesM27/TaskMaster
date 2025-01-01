using Microsoft.EntityFrameworkCore;
using TaskMaster.Modules.Teaching.Entities;

namespace TaskMaster.Modules.Teaching.DAL;

internal sealed class TeachingDbContext(DbContextOptions<TeachingDbContext> options)
    : DbContext(options)
{
    public DbSet<Assignment> Assignments { get; set; }
    public DbSet<AssignmentExercise> AssignmentExercises { get; set; }
    public DbSet<ClassAssignment> ClassAssignments { get; set; }
    public DbSet<EssayAnswer> EssayAnswers { get; set; }
    public DbSet<MailAnswer> MailAnswers { get; set; }
    public DbSet<School> Schools { get; set; }
    public DbSet<SchoolAdmin> SchoolAdmins { get; set; }
    public DbSet<SchoolTeacher> SchoolTeachers { get; set; }
    public DbSet<SummaryOfTextAnswer> SummaryOfTextAnswers { get; set; }
    public DbSet<TeachingClass> TeachingClasses { get; set; }
        
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Teaching");
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}