﻿using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskMaster.Modules.Accounts.Entities;

namespace TaskMaster.Modules.Accounts.DAL.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        var serializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Email).IsRequired().HasMaxLength(400).IsUnicode();
        builder.Property(x => x.PasswordHash).IsRequired();
        builder.Property(x => x.PasswordSalt).IsRequired();
        builder.Property(x => x.Firstname).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Lastname).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Role).IsRequired().HasMaxLength(200);
        builder.Property(x => x.IsActive).IsRequired();
        builder.Property(x => x.ActivationToken).IsRequired(false).HasMaxLength(200);
        builder.Property(x => x.ActivationTokenExpires).IsRequired(false);
        builder.Property(x => x.PasswordResetToken).IsRequired(false).HasMaxLength(200);
        builder.Property(x => x.PasswordTokenExpires).IsRequired(false);
        builder.Property(x => x.Banned).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.Claims).IsRequired()
            .HasConversion(x => JsonSerializer.Serialize(x, serializerOptions),
                x => JsonSerializer.Deserialize<Dictionary<string, IEnumerable<string>>>(x, serializerOptions)!);
        builder.HasIndex(x => x.Email).IsUnique();
        builder.HasIndex(x => x.PasswordResetToken).IsUnique();

        builder.Property(x => x.Claims).Metadata.SetValueComparer(
            new ValueComparer<Dictionary<string, IEnumerable<string>>>(
                (c1, c2) => c1!.SequenceEqual(c2!),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToDictionary(x => x.Key, x => x.Value)));
    }
}