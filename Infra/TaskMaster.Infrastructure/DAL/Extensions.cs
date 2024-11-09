using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TaskMaster.Infrastructure.Options;
using TaskMaster.Infrastructure.Settings;

namespace TaskMaster.Infrastructure.DAL;

public static class Extensions
{
    public static IServiceCollection AddPostgres<TDbContext, TDbInitializer>(this IServiceCollection services)
        where TDbContext : DbContext where TDbInitializer : class, IHostedService
    {
        var options = services.GetOptions<PostgresSettings>(PostgresSettings.SectionName);
        services.AddDbContext<TDbContext>(x => x.UseNpgsql(options.ConnectionString));
        services.AddHostedService<TDbInitializer>();
        return services;
    }
}