using Microsoft.Extensions.DependencyInjection;
using TaskMaster.Abstractions.Commands;

namespace TaskMaster.Infrastructure.Commands;

internal sealed class CommandDispatcher(IServiceProvider serviceProvider) : ICommandDispatcher
{
    public async Task SendAsync<TCommand>(TCommand command) where TCommand : class, ICommand
    {
        if (command is null)
            return;

        using var scope = serviceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<ICommandHandler<TCommand>>();
        await handler.HandleAsync(command);
    }
}