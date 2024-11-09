using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenAI_API;
using TaskMaster.Infrastructure.Options;
using TaskMaster.Infrastructure.Settings;
using TaskMaster.OpenAi.OpenForm.Endpoints;
using TaskMaster.OpenAi.Services;

namespace TaskMaster.OpenAi;

public static class ModuleExtensions
{
    public static IServiceCollection AddOpenAi(this IServiceCollection services)
    {
        var openAiSettings = services.GetOptions<OpenAiSettings>(OpenAiSettings.SectionName);
        services.AddScoped<IOpenAIAPI>(_ => new OpenAIAPI(openAiSettings.ApiKey));

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