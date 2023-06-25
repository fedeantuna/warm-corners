using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using WarmCorners.Core.Common;
using WarmCorners.Core.Services.Abstractions;
using WarmCorners.Service.Infrastructure.Wrapper;

namespace WarmCorners.Service.Tests.Unit.Infrastructure.Services;

public class ScreenServiceTests
{
    private const int ScreenWidth = 1024;
    private const int ScreenHeight = 768;

    private readonly IScreenService _screenResolutionProvider;

    public ScreenServiceTests()
    {
        var provider = new ServiceProviderBuilder().Build();

        var user32Wrapper = provider.GetRequiredService<IUser32Wrapper>();
        var user32WrapperMock = Mock.Get(user32Wrapper);
        user32WrapperMock.Setup(u32W => u32W.GetScreenResolution()).Returns((ScreenWidth, ScreenHeight));

        this._screenResolutionProvider = provider.GetRequiredService<IScreenService>();
    }

    [Theory]
    [InlineData(ScreenCorner.TopLeft, -1, -1, true)]
    [InlineData(ScreenCorner.TopLeft, -1, 0, true)]
    [InlineData(ScreenCorner.TopLeft, -1, 1, false)]
    [InlineData(ScreenCorner.TopLeft, 0, -1, true)]
    [InlineData(ScreenCorner.TopLeft, 0, 0, true)]
    [InlineData(ScreenCorner.TopLeft, 0, 1, false)]
    [InlineData(ScreenCorner.TopLeft, 1, -1, false)]
    [InlineData(ScreenCorner.TopLeft, 1, 0, false)]
    [InlineData(ScreenCorner.TopLeft, 1, 1, false)]
    [InlineData(ScreenCorner.TopRight, ScreenWidth - 1, -1, false)]
    [InlineData(ScreenCorner.TopRight, ScreenWidth - 1, 0, false)]
    [InlineData(ScreenCorner.TopRight, ScreenWidth - 1, 1, false)]
    [InlineData(ScreenCorner.TopRight, ScreenWidth, -1, true)]
    [InlineData(ScreenCorner.TopRight, ScreenWidth, 0, true)]
    [InlineData(ScreenCorner.TopRight, ScreenWidth, 1, false)]
    [InlineData(ScreenCorner.TopRight, ScreenWidth + 1, -1, true)]
    [InlineData(ScreenCorner.TopRight, ScreenWidth + 1, 0, true)]
    [InlineData(ScreenCorner.TopRight, ScreenWidth + 1, 1, false)]
    [InlineData(ScreenCorner.BottomRight, ScreenWidth - 1, ScreenHeight - 1, false)]
    [InlineData(ScreenCorner.BottomRight, ScreenWidth - 1, ScreenHeight, false)]
    [InlineData(ScreenCorner.BottomRight, ScreenWidth - 1, ScreenHeight + 1, false)]
    [InlineData(ScreenCorner.BottomRight, ScreenWidth, ScreenHeight - 1, false)]
    [InlineData(ScreenCorner.BottomRight, ScreenWidth, ScreenHeight, true)]
    [InlineData(ScreenCorner.BottomRight, ScreenWidth, ScreenHeight + 1, true)]
    [InlineData(ScreenCorner.BottomRight, ScreenWidth + 1, ScreenHeight - 1, false)]
    [InlineData(ScreenCorner.BottomRight, ScreenWidth + 1, ScreenHeight, true)]
    [InlineData(ScreenCorner.BottomRight, ScreenWidth + 1, ScreenHeight + 1, true)]
    [InlineData(ScreenCorner.BottomLeft, -1, ScreenHeight - 1, false)]
    [InlineData(ScreenCorner.BottomLeft, -1, ScreenHeight, true)]
    [InlineData(ScreenCorner.BottomLeft, -1, ScreenHeight + 1, true)]
    [InlineData(ScreenCorner.BottomLeft, 0, ScreenHeight - 1, false)]
    [InlineData(ScreenCorner.BottomLeft, 0, ScreenHeight, true)]
    [InlineData(ScreenCorner.BottomLeft, 0, ScreenHeight + 1, true)]
    [InlineData(ScreenCorner.BottomLeft, 1, ScreenHeight - 1, false)]
    [InlineData(ScreenCorner.BottomLeft, 1, ScreenHeight, false)]
    [InlineData(ScreenCorner.BottomLeft, 1, ScreenHeight + 1, false)]
    public void GetsTheCurrentCornerThresholdsForTheCurrentScreenResolution(ScreenCorner screenCorner, int x, int y, bool expectedResult)
    {
        // Act
        var result = this._screenResolutionProvider.IsMouseCursorInCorner(screenCorner, x, y);

        // Assert
        result.Should().Be(expectedResult);
    }
}
