using Microsoft.Extensions.DependencyInjection;
using TaskMaster.Infrastructure.DAL;
using TaskMaster.Modules.Exercises.DAL;
using TaskMaster.Modules.Exercises.DAL.Repositories;
using TaskMaster.Modules.Exercises.Repositories;

namespace TaskMaster.Modules.Exercises;

public static class Extensions
{
    public static IServiceCollection AddExercisesModule(this IServiceCollection services)
    {
        services
            .AddPostgres<ExercisesDbContext, DatabaseInitializer>()
            .AddScoped<IOpenFormRepository, OpenFormRepository>();
        return services;
    }
}