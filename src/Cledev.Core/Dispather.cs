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
    private readonly IObjectFactory _objectFactory;

    public Dispatcher(ICommandSender commandSender, IQueryProcessor queryProcessor, IEventPublisher eventPublisher, IStreamCreator streamCreator, IObjectFactory objectFactory)
    {
        _commandSender = commandSender;
        _queryProcessor = queryProcessor;
        _eventPublisher = eventPublisher;
        _streamCreator = streamCreator;
        _objectFactory = objectFactory;
    }

    public async Task<Result> Send<TCommand>(TCommand command, CancellationToken cancellationToken = default) where TCommand : ICommand
    {
        var commandResult = await _commandSender.Send(command, cancellationToken);

        return await commandResult.Match(HandleSuccess, HandleFailure);

        async Task<Result> HandleSuccess(Success success)
        {
            var events = success.Events.ToList();
            if (events.Any() is false)
            {
                return success;
            }

            var tasks = new List<Task<Result>>();

            foreach (var @event in events)
            {
                var concreteEvent = _objectFactory.CreateConcreteObject(@event);
                var task = _eventPublisher.Publish(concreteEvent, cancellationToken);
                tasks.Add(task);
            }

            var results = await Task.WhenAll(tasks);
            if (results.Any(result => result.IsFailure))
            {
                // TODO: Handle event publisher failed results
            }

            return success;
        }

        async Task<Result> HandleFailure(Failure failure)
        {
            return await Task.FromResult(failure);
        }
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
