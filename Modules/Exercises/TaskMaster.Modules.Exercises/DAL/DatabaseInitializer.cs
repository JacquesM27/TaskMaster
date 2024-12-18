using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TaskMaster.Modules.Exercises.DAL;

internal sealed class DatabaseInitializer(
    IServiceProvider serviceProvider,
    ILogger<DatabaseInitializer> logger) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ExercisesDbContext>();
        dbContext.Database.Migrate();
        logger.LogInformation("Exercises database migrated");

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}