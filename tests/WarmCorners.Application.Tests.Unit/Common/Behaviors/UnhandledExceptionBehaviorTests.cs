using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Sinks.InMemory;
using Serilog.Sinks.InMemory.Assertions;
using WarmCorners.Application.Common.Behaviors;
using WarmCorners.Application.Tests.Unit.Fakes;

namespace WarmCorners.Application.Tests.Unit.Common.Behaviors;

public class UnhandledExceptionBehaviorTests
{
    private readonly ISender _sender;

    public UnhandledExceptionBehaviorTests()
    {
        var provider = new ServiceProviderBuilder().Build();

        this._sender = provider.GetRequiredService<ISender>();
    }

    [Fact]
    public async Task LogsErrorWhenRequestFails()
    {
        // Arrange
        var request = new FailingRequestFake();
        var cancellationToken = default(CancellationToken);

        // Act
        var act = () => this._sender.Send(request, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<Exception>();
        InMemorySink.Instance
            .Should()
            .HaveMessage(UnhandledExceptionBehavior<IRequest, Exception>.UnhandledExceptionLogMessageTemplate).Once()
            .WithProperty("RequestName").WithValue(nameof(FailingRequestFake))
            .And.WithProperty("Request").HavingADestructuredObject();
    }
}
