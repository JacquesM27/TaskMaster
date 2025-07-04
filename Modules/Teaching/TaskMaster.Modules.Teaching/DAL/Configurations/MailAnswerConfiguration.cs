﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskMaster.Modules.Teaching.Entities.OpenForm;

namespace TaskMaster.Modules.Teaching.DAL.Configurations;

internal sealed class MailAnswerConfiguration : IEntityTypeConfiguration<MailAnswer>
{
    public void Configure(EntityTypeBuilder<MailAnswer> builder)
    {
        builder.HasKey(ma => ma.Id);

        builder.OwnsMany(ma => ma.Mistakes, m =>
        {
            m.WithOwner().HasForeignKey("MailAnswerId");
            m.ToTable("MailAnswerMistakes");
        });
    }
}