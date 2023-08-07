using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SharpHook;
using SharpHook.Native;
using WarmCorners.Application.KeyCombinationTriggers.Commands.TriggerKeyCombination;

namespace WarmCorners.Tests.Integration.KeyCombinationTriggers.Commands;

public class ProcessKeyCombinationTriggerCommandTests
{
    private Mock<IEventSimulator> _eventSimulatorMock = null!;
    private ISender _sender = null!;

    [SetUp]
    public void SetUp()
    {
        var provider = new ServiceProviderBuilder().Build();

        this._sender = provider.GetRequiredService<ISender>();

        var eventSimulator = provider.GetRequiredService<IEventSimulator>();
        this._eventSimulatorMock = Mock.Get(eventSimulator);
    }

    [Test]
    public async Task ExecutesKeyCombinationWhenUserMovesMouseOverCorner()
    {
        // Arrange
        var expectedKeyCombination = new List<KeyCode>
        {
            KeyCode.VcLeftMeta,
            KeyCode.VcTab
        };

        // Act
        await this._sender.Send(new TriggerKeyCombinationCommand
        {
            KeyCombination = expectedKeyCombination
        });

        // Assert
        this.VerifyCommandIsExecutedOnce(expectedKeyCombination);
    }

    private void VerifyCommandIsExecutedOnce(IReadOnlyCollection<KeyCode> keyCombination)
    {
        keyCombination.ToList().ForEach(k =>
            this._eventSimulatorMock.Verify(es =>
                es.SimulateKeyPress(k), Times.Once));
        keyCombination.ToList().ForEach(k =>
            this._eventSimulatorMock.Verify(es =>
                es.SimulateKeyRelease(k), Times.Once));
    }
}
