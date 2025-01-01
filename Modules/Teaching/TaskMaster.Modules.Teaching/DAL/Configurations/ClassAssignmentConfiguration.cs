using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskMaster.Modules.Teaching.Entities;

namespace TaskMaster.Modules.Teaching.DAL.Configurations;

public class ClassAssignmentConfiguration : IEntityTypeConfiguration<ClassAssignment>
{
    public void Configure(EntityTypeBuilder<ClassAssignment> builder)
    {
        builder.HasKey(ca => ca.Id);
        
        builder.HasOne(ca => ca.TeachingClass)
            .WithMany(tc => tc.Assignments)
            .HasForeignKey(ca => ca.TeachingClassId);
        
        builder.HasOne(ca => ca.Assignment)
            .WithMany()
            .HasForeignKey(ca => ca.AssignmentId);
    }
}