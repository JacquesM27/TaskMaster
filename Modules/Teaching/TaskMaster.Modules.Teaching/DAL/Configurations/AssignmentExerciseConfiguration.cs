using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskMaster.Modules.Teaching.Entities;

namespace TaskMaster.Modules.Teaching.DAL.Configurations;

public class AssignmentExerciseConfiguration : IEntityTypeConfiguration<AssignmentExercise>
{
    public void Configure(EntityTypeBuilder<AssignmentExercise> builder)
    {
        builder.HasKey(ae => ae.Id);
        
        builder.HasIndex(ae => new { ae.AssignmentId, ae.ExerciseId });
        builder.HasOne(ae => ae.Assignment)
            .WithMany(ae => ae.Exercises)
            .HasForeignKey(ae => ae.AssignmentId);
    }
}