using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using TaskMaster.Infrastructure.DAL;
using TaskMaster.Modules.Exercises.DAL;
using TaskMaster.Modules.Exercises.DAL.Repositories;
using TaskMaster.Modules.Exercises.OpenForm.Endpoints;
using TaskMaster.Modules.Exercises.Repositories;
using TaskMaster.Modules.Exercises.Services;

namespace TaskMaster.Modules.Exercises;

public static class Extensions
{
    public static IServiceCollection AddExercisesModule(this IServiceCollection services)
    {
        services
            .AddPostgres<ExercisesDbContext, DatabaseInitializer>()
            .AddScoped<IOpenFormRepository, OpenFormRepository>()
            .AddTransient<IOpenFormService, OpenFormService>();
        return services;
    }
    
    public static WebApplication UseOpenAi(this WebApplication app)
    {
        app.AddOpenFormEndpoints();

        return app;
    }
}