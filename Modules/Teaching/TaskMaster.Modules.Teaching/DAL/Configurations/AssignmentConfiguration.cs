using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskMaster.Modules.Teaching.Entities;

namespace TaskMaster.Modules.Teaching.DAL.Configurations;

internal sealed class AssignmentConfiguration : IEntityTypeConfiguration<Assignment>
{
    public void Configure(EntityTypeBuilder<Assignment> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Name).IsRequired().HasMaxLength(100);
        builder.Property(a => a.Description).IsRequired().HasMaxLength(500);
        builder.Property(a => a.DueDate).IsRequired();
        builder.Property(a => a.Password).IsRequired(false).HasMaxLength(100);
        builder.HasMany(a => a.Exercises)
            .WithOne(a => a.Assignment)
            .HasForeignKey(a => a.AssignmentId);
    }
}