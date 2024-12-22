using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskMaster.Modules.Teaching.Entities;

namespace TaskMaster.Modules.Teaching.DAL.Configurations;

public class SchoolTeacherConfiguration : IEntityTypeConfiguration<SchoolTeacher>
{
    public void Configure(EntityTypeBuilder<SchoolTeacher> builder)
    {
        builder.HasKey(st => new {st.SchoolId, st.TeacherId});
        
        builder.HasOne(st => st.School)
            .WithMany(s => s.SchoolTeachers)
            .HasForeignKey(st => st.SchoolId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}