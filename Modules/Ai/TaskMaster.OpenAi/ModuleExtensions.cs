using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using TaskMaster.OpenAi.OpenForm.Endpoints;
using TaskMaster.OpenAi.Services;

namespace TaskMaster.OpenAi;

public static class ModuleExtensions
{
    public static IServiceCollection AddOpenAi(this IServiceCollection services)
    {
        services.AddTransient<IPromptFormatter, PromptFormatter>();
        services.AddTransient<IObjectSamplerService, ObjectSamplerService>();
        services.AddScoped<IOpenAiExerciseService, OpenAiExerciseService>();

        return services;
    }

    public static WebApplication UseOpenAi(this WebApplication app)
    {
        app.AddOpenFormEndpoints();

        return app;
    }
}