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

    public async Task<Result> Send<TCommand>(TCommand? command, CancellationToken cancellationToken = default) where TCommand : ICommand
    {
        if (command is null)
        {
            return Result.Fail(ErrorCodes.Error, title: "Null Argument", description: $"Command of type {typeof(TCommand)} is null");
        }

        var handler = _serviceProvider.GetService<ICommandHandler<TCommand>>();

        if (handler is null)
        {
            return Result.Fail(ErrorCodes.Error, title: "Handler not found", description: $"Handler not found for command of type {typeof(TCommand)}");
        }

        return await handler.Handle(command, cancellationToken);
    }
}
