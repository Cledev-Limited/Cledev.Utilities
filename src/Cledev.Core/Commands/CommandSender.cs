using Cledev.Core.Results;
using Microsoft.Extensions.DependencyInjection;

namespace Cledev.Core.Commands;

public class CommandSender : ICommandSender
{
    private readonly IServiceProvider _serviceProvider;

    public CommandSender(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<Result> Send<TCommand>(TCommand command, CancellationToken cancellationToken = default) where TCommand : ICommand
    {
        if (command is null)
        {
            throw new ArgumentNullException(nameof(command));
        }
        
        var handler = _serviceProvider.GetService<ICommandHandler<TCommand>>();

        if (handler is null)
        {
            throw new Exception("No command handler found for command.");
        }

        return await handler.Handle(command, cancellationToken);
    }
}
