using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskMaster.Modules.Teaching.Entities;

namespace TaskMaster.Modules.Teaching.DAL.Configurations;

internal sealed class TeachingClassConfiguration : IEntityTypeConfiguration<TeachingClass>
{
    public void Configure(EntityTypeBuilder<TeachingClass> builder)
    {
        builder.HasKey(tc => tc.Id);
        builder.Property(tc => tc.Name).IsRequired().HasMaxLength(100);
        builder.Property(tc => tc.Level).IsRequired().HasMaxLength(50);
        builder.Property(tc => tc.Language).IsRequired().HasMaxLength(50);
        builder.Property(tc => tc.MainTeacherId).IsRequired();
        builder.Property(tc => tc.SubTeachersIds).IsRequired();
        builder.Property(tc => tc.StudentsIds).IsRequired();
        builder.Property(tc => tc.SchoolId).IsRequired();
        
        builder.HasOne(tc => tc.School)
            .WithMany()
            .HasForeignKey(tc => tc.SchoolId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder.HasMany(tc => tc.Assignments)
            .WithOne(ca => ca.TeachingClass)
            .HasForeignKey(ca => ca.TeachingClassId)
            //.OnDelete(DeleteBehavior.Cascade)
            ;
    }
}