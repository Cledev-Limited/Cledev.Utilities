using Cledev.Core.Commands;
using Cledev.Core.Events;
using Cledev.Core.Mapping;
using Cledev.Core.Queries;
using Cledev.Core.Results;
using Cledev.Core.Streams;

namespace Cledev.Core;

public class Dispatcher : IDispatcher
{
    private readonly ICommandSender _commandSender;
    private readonly IQueryProcessor _queryProcessor;
    private readonly IEventPublisher _eventPublisher;
    private readonly IStreamCreator _streamCreator;

    public Dispatcher(ICommandSender commandSender, IQueryProcessor queryProcessor, IEventPublisher eventPublisher, IStreamCreator streamCreator)
    {
        _commandSender = commandSender;
        _queryProcessor = queryProcessor;
        _eventPublisher = eventPublisher;
        _streamCreator = streamCreator;
    }

    public async Task<Result> Send<TCommand>(TCommand command, CancellationToken cancellationToken = default) where TCommand : ICommand
    {
        return await _commandSender.Send(command, cancellationToken);
    }

    public async Task<Result<TResult>> Get<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
    {
        return await _queryProcessor.Process(query, cancellationToken);
    }

    public async Task<Result> Publish<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IEvent
    {
        return await _eventPublisher.Publish(@event, cancellationToken);
    }

    public IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        return _streamCreator.Create(request, cancellationToken);
    }
}
