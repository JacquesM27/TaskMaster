using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskMaster.Modules.Teaching.Entities;

namespace TaskMaster.Modules.Teaching.DAL.Configurations;

public class SchoolConfiguration : IEntityTypeConfiguration<School>
{
    public void Configure(EntityTypeBuilder<School> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Name).IsRequired().HasMaxLength(300);
        
        builder.HasMany(s => s.Classes)
            .WithOne(c => c.School)
            .HasForeignKey(c => c.SchoolId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(s => s.SchoolTeachers)
            .WithOne(st => st.School)
            .HasForeignKey(st => st.SchoolId)
            .OnDelete(DeleteBehavior.Cascade);
        
    }
}