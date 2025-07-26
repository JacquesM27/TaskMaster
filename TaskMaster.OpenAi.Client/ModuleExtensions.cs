using Microsoft.Extensions.DependencyInjection;
using TaskMaster.OpenAi.Client.Clients;

namespace TaskMaster.OpenAi.Client;

public static class ModuleExtensions
{
    public static IServiceCollection AddOpenAiClient(this IServiceCollection services)
    {
        services.AddTransient<IOpenAiExerciseClient, OpenAiExerciseClient>();

        return services;
    }
}