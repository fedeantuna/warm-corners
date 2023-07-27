using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using WarmCorners.Application.Common.Wrappers;
using WarmCorners.Application.ShellTriggers.Commands;
using WarmCorners.Domain.Enums;

namespace WarmCorners.Tests.Integration.ShellTriggers.Commands;

public class ProcessShellTriggerCommandTests
{
    private Mock<IProcessWrapper> _processWrapperMock = null!;
    private ISender _sender = null!;

    [SetUp]
    public void SetUp()
    {
        var provider = new ServiceProviderBuilder().Build();

        this._sender = provider.GetRequiredService<ISender>();

        var processWrapper = provider.GetRequiredService<IProcessWrapper>();
        this._processWrapperMock = Mock.Get(processWrapper);
    }

    [Test]
    public async Task ExecutesCommandWhenUserMovesMouseOverCorner()
    {
        // Arrange
        const string expectedCommand = "notepad";

        // Act
        await this._sender.Send(new ProcessShellTriggerCommand
        {
            Position = (Testing.TopRightCorner.X, Testing.TopRightCorner.Y),
            ScreenCorner = ScreenCorner.TopRight,
            ShellCommand = expectedCommand
        });

        // Assert
        this.VerifyCommandIsExecutedOnce(expectedCommand);
    }

    private void VerifyCommandIsExecutedOnce(string command)
    {
        this._processWrapperMock.Verify(pw =>
            pw.SetStartInfo(It.Is<ProcessStartInfo>(psi =>
                psi.Arguments == $"/c {command}")), Times.Once);
        this._processWrapperMock.Verify(pw =>
            pw.Start(), Times.Once);
    }
}
