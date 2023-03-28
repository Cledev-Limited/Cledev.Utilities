using Cledev.Core.Commands;
using Cledev.Core.Events;
using Cledev.Core.Queries;
using Cledev.Core.Results;

namespace Cledev.Core;

public interface IDispatcher
{
    Task<Result> Send<TCommand>(TCommand command, CancellationToken cancellationToken = default) where TCommand : ICommand;
    Task<Result<TResult>> Get<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default);
    Task<Result> Publish<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IEvent;
    
    // TODO: Add support for stream requests
}
