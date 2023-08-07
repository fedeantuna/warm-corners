using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Sinks.InMemory;
using Serilog.Sinks.InMemory.Assertions;
using WarmCorners.Application.Common.Behaviors;
using WarmCorners.Application.Tests.Unit.Fakes;

namespace WarmCorners.Application.Tests.Unit.Common.Behaviors;

public class LoggingBehaviorTests
{
    private readonly ISender _sender;

    public LoggingBehaviorTests()
    {
        var provider = new ServiceProviderBuilder().Build();

        this._sender = provider.GetRequiredService<ISender>();
    }

    [Fact]
    public async Task LogsInformationAboutTheUserAndTheRequest()
    {
        // Arrange
        var request = new UnvalidatedPassingRequestFake();
        var cancellationToken = default(CancellationToken);

        // Act
        await this._sender.Send(request, cancellationToken);

        // Assert
        InMemorySink.Instance
            .Should()
            .HaveMessage(LoggingBehavior<UnvalidatedPassingRequestFake>.RequestLogMessageTemplate).Once()
            .WithProperty("RequestName").WithValue(nameof(UnvalidatedPassingRequestFake))
            .And.WithProperty("Request").HavingADestructuredObject();
    }
}
