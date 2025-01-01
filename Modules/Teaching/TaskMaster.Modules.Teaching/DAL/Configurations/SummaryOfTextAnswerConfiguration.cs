using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskMaster.Modules.Teaching.Entities;

namespace TaskMaster.Modules.Teaching.DAL.Configurations;

public class SummaryOfTextAnswerConfiguration : IEntityTypeConfiguration<SummaryOfTextAnswer>
{
    public void Configure(EntityTypeBuilder<SummaryOfTextAnswer> builder)
    {
        builder.HasKey(sta => sta.Id);

        builder.OwnsMany(sta => sta.Mistakes, m =>
        {
            m.WithOwner().HasForeignKey("SummaryOfTextAnswerId");
            m.ToTable("SummaryOfTextAnswerMistakes");
        });
    }
}