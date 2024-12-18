using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskMaster.Modules.Exercises.Entities;

namespace TaskMaster.Modules.Exercises.DAL.Configurations;

internal sealed class SummaryOfTextConfiguration : IEntityTypeConfiguration<SummaryOfText>
{
    public void Configure(EntityTypeBuilder<SummaryOfText> builder)
    {
        builder.HasKey(m => m.Id);
        builder.OwnsOne(m => m.Exercise, onb =>
        {
            onb.ToJson();
            onb.OwnsOne(exercise => exercise.Header);
        });
        builder.Property(x => x.ExerciseHeaderInMotherLanguage).IsRequired();
        builder.Property(x => x.MotherLanguage).IsRequired().HasMaxLength(50);
        builder.Property(x => x.TargetLanguage).IsRequired().HasMaxLength(50);
        builder.Property(x => x.TargetLanguageLevel).IsRequired().HasMaxLength(50);
        builder.Property(x => x.TopicsOfSentences).IsRequired(false).HasMaxLength(200);
        builder.Property(x => x.GrammarSection).IsRequired(false).HasMaxLength(200);
        builder.Property(x => x.VerifiedByTeacher).IsRequired();
    }
}