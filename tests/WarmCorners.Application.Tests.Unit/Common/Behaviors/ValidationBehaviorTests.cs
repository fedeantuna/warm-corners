using FluentAssertions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using WarmCorners.Application.Tests.Unit.Fakes;

namespace WarmCorners.Application.Tests.Unit.Common.Behaviors;

public class ValidationBehaviorTests
{
    private readonly ISender _sender;

    public ValidationBehaviorTests()
    {
        var provider = new ServiceProviderBuilder().Build();

        this._sender = provider.GetRequiredService<ISender>();
    }

    [Fact]
    public async Task ShouldNotThrowAnyExceptionWhenValidationsArePassing()
    {
        // Arrange
        var request = new ValidatedPassingRequestFake
        {
            SomeString = "some-string"
        };
        var cancellationToken = default(CancellationToken);

        // Act
        var act = () => this._sender.Send(request, cancellationToken);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task ShouldNotThrowAnyExceptionWhenThereAreNoValidations()
    {
        // Arrange
        var request = new UnvalidatedPassingRequestFake();
        var cancellationToken = default(CancellationToken);

        // Act
        var act = () => this._sender.Send(request, cancellationToken);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task ShouldThrowValidationExceptionWhenTheValidationFails()
    {
        // Arrange
        var request = new ValidatedPassingRequestFake
        {
            SomeString = null
        };
        var cancellationToken = default(CancellationToken);

        // Act
        var act = () => this._sender.Send(request, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }
}
