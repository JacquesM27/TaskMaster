using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskMaster.Modules.Teaching.Entities;

namespace TaskMaster.Modules.Teaching.DAL.Configurations;

internal sealed class SchoolAdminConfiguration : IEntityTypeConfiguration<SchoolAdmin>
{
    public void Configure(EntityTypeBuilder<SchoolAdmin> builder)
    {
        builder.HasKey(sa => new {sa.SchoolId, sa.AdminId});
        
        builder.HasOne(sa => sa.School)
            .WithMany(s => s.SchoolAdmins)
            .HasForeignKey(sa => sa.SchoolId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}