using Microsoft.Extensions.DependencyInjection;
using TaskMaster.Infrastructure.DAL;
using TaskMaster.Modules.Teaching.DAL;

namespace TaskMaster.Modules.Teaching;

public static class Extensions
{
    public static IServiceCollection AddTeachingModule(this IServiceCollection services)
    {
        services.AddPostgres<TeachingDbContext, DatabaseInitializer>();

        return services;
    }
}