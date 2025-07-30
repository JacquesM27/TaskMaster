using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TaskMaster.Infrastructure.DAL;

internal sealed class CommunicationDatabaseInitializer(
    IServiceProvider serviceProvider,
    ILogger<CommunicationDatabaseInitializer> logger) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TaskMasterCommunicationDbContext>();
        dbContext.Database.Migrate();
        logger.LogInformation("Communication database migrated");

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}