using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskMaster.Modules.Teaching.Entities;

namespace TaskMaster.Modules.Teaching.DAL.Configurations;

public class EssayAnswerConfiguration : IEntityTypeConfiguration<EssayAnswer>
{
    public void Configure(EntityTypeBuilder<EssayAnswer> builder)
    {
        builder.HasKey(ea => ea.Id);

        builder.OwnsMany(ea => ea.Mistakes, m =>
        {
            m.WithOwner().HasForeignKey("EssayAnswerId");
            m.ToTable("EssayAnswerMistakes");
        });
    }
}