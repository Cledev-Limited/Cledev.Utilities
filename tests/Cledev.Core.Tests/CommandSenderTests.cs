using Cledev.Core.Commands;
using Cledev.Core.Results;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Cledev.Core.Tests;

[TestFixture]
public class CommandSenderTests
{
    private ICommandSender _commandSender;
    private Mock<IServiceProvider> _serviceProviderMock;

        [SetUp]
    public void Setup()
    {
        _serviceProviderMock = new Mock<IServiceProvider>();
        _commandSender = new CommandSender(_serviceProviderMock.Object);
    }
    
    [Test]
    public async Task GivenCommandIsNull_ThenExceptionShouldBeThrown()
    {
        var action = async () => { await _commandSender.Send(default(TestCommand)); };
        await action.Should().ThrowAsync<Exception>();
    }
    
    [Test]
    public async Task GivenHandlerNotFound_ThenExceptionShouldBeThrown()
    {
        var action = async () => { await _commandSender.Send(new TestCommand()); };
        await action.Should().ThrowAsync<Exception>();
    }
    
    [Test]
    public async Task GivenValidCommandAndHandler_ThenSuccessResultIsReturned()
    {
        _serviceProviderMock
            .Setup(x => x.GetService(typeof(ICommandHandler<TestCommand>)))
            .Returns(new TestCommandHandler());
        var result = await _commandSender.Send(new TestCommand());
        result.Should().Be(Result.Ok());
    }
}

public record TestCommand : ICommand;

public class TestCommandHandler : ICommandHandler<TestCommand>
{
    public async Task<Result> Handle(TestCommand command, CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
        return Result.Ok();
    }
}
