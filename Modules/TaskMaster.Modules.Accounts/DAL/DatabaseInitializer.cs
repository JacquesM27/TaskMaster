﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TaskMaster.Modules.Accounts.Entities;
using TaskMaster.Modules.Accounts.Services;

namespace TaskMaster.Modules.Accounts.DAL;

internal sealed class DatabaseInitializer(
    IServiceProvider serviceProvider,
    TimeProvider timeProvider,
    IPasswordManager passwordManager,
    ILogger<DatabaseInitializer> logger) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
            dbContext.Database.Migrate();
            logger.LogInformation("Database migrated");

            var users = dbContext.Users.ToList();
            if (users.Count is not 0)
                return Task.CompletedTask;

            passwordManager.CreatePasswordHash("SuperSecretAdminPassword123!", out var adminHash, out var adminSalt);
            var admin = new User
            {
                Id = Guid.NewGuid(),
                Email = "admin@taskmaster.com",
                PasswordHash = adminHash,
                PasswordSalt = adminSalt,
                Firstname = "CarHub",
                Lastname = "Admin",
                Role = "Admin",
                IsActive = true,
                CreatedAt = timeProvider.GetLocalNow().DateTime,
                Claims = []
            };

            passwordManager.CreatePasswordHash("WeakPassword99#", out var userHash, out var userSalt);
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "user@taskmaster.com",
                PasswordHash = userHash,
                PasswordSalt = userSalt,
                Firstname = "Ryan",
                Lastname = "Reynolds",
                Role = "User",
                IsActive = true,
                CreatedAt = timeProvider.GetLocalNow().DateTime,
                Claims = []
            };

            users = [admin, user];

            dbContext.Users.AddRange(users);
            dbContext.SaveChanges();
            logger.LogInformation("Database initialized");
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}