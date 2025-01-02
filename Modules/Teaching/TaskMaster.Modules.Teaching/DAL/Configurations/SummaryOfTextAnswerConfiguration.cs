using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskMaster.Modules.Teaching.Entities;
using TaskMaster.Modules.Teaching.Entities.OpenForm;

namespace TaskMaster.Modules.Teaching.DAL.Configurations;

internal sealed class SummaryOfTextAnswerConfiguration : IEntityTypeConfiguration<SummaryOfTextAnswer>
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