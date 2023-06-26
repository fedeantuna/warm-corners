using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using WarmCorners.Core.Common;
using WarmCorners.Core.Services.Abstractions;

namespace WarmCorners.Service.Tests.Unit.Infrastructure.Services;

public class ScreenServiceTests
{
    public static readonly TheoryData<ScreenCorner, int, int, bool> TopLeftCornerTestCases = new()
    {
        // ScreenCorner         Mouse Cursor Position (X)    Mouse Cursor Position (Y)    Expected
        { ScreenCorner.TopLeft, Testing.TopLeftCorner.X - 1, Testing.TopLeftCorner.Y - 1, true },
        { ScreenCorner.TopLeft, Testing.TopLeftCorner.X - 1, Testing.TopLeftCorner.Y + 0, true },
        { ScreenCorner.TopLeft, Testing.TopLeftCorner.X - 1, Testing.TopLeftCorner.Y + 1, false },
        { ScreenCorner.TopLeft, Testing.TopLeftCorner.X + 0, Testing.TopLeftCorner.Y - 1, true },
        { ScreenCorner.TopLeft, Testing.TopLeftCorner.X + 0, Testing.TopLeftCorner.Y + 0, true },
        { ScreenCorner.TopLeft, Testing.TopLeftCorner.X + 0, Testing.TopLeftCorner.Y + 1, false },
        { ScreenCorner.TopLeft, Testing.TopLeftCorner.X + 1, Testing.TopLeftCorner.Y - 1, false },
        { ScreenCorner.TopLeft, Testing.TopLeftCorner.X + 1, Testing.TopLeftCorner.Y + 0, false },
        { ScreenCorner.TopLeft, Testing.TopLeftCorner.X + 1, Testing.TopLeftCorner.Y + 1, false }
    };

    public static readonly TheoryData<ScreenCorner, int, int, bool> TopRightCornerTestCases = new()
    {
        // ScreenCorner          Mouse Cursor Position (X)     Mouse Cursor Position (Y)     Expected
        { ScreenCorner.TopRight, Testing.TopRightCorner.X - 1, Testing.TopRightCorner.Y - 1, false },
        { ScreenCorner.TopRight, Testing.TopRightCorner.X - 1, Testing.TopRightCorner.Y + 0, false },
        { ScreenCorner.TopRight, Testing.TopRightCorner.X - 1, Testing.TopRightCorner.Y + 1, false },
        { ScreenCorner.TopRight, Testing.TopRightCorner.X + 0, Testing.TopRightCorner.Y - 1, true },
        { ScreenCorner.TopRight, Testing.TopRightCorner.X + 0, Testing.TopRightCorner.Y + 0, true },
        { ScreenCorner.TopRight, Testing.TopRightCorner.X + 0, Testing.TopRightCorner.Y + 1, false },
        { ScreenCorner.TopRight, Testing.TopRightCorner.X + 1, Testing.TopRightCorner.Y - 1, true },
        { ScreenCorner.TopRight, Testing.TopRightCorner.X + 1, Testing.TopRightCorner.Y + 0, true },
        { ScreenCorner.TopRight, Testing.TopRightCorner.X + 1, Testing.TopRightCorner.Y + 1, false }
    };

    public static readonly TheoryData<ScreenCorner, int, int, bool> BottomRightCornerTestCases = new()
    {
        // ScreenCorner             Mouse Cursor Position (X)        Mouse Cursor Position (Y)        Expected
        { ScreenCorner.BottomRight, Testing.BottomRightCorner.X - 1, Testing.BottomRightCorner.Y - 1, false },
        { ScreenCorner.BottomRight, Testing.BottomRightCorner.X - 1, Testing.BottomRightCorner.Y + 0, false },
        { ScreenCorner.BottomRight, Testing.BottomRightCorner.X - 1, Testing.BottomRightCorner.Y + 1, false },
        { ScreenCorner.BottomRight, Testing.BottomRightCorner.X + 0, Testing.BottomRightCorner.Y - 1, false },
        { ScreenCorner.BottomRight, Testing.BottomRightCorner.X + 0, Testing.BottomRightCorner.Y + 0, true },
        { ScreenCorner.BottomRight, Testing.BottomRightCorner.X + 0, Testing.BottomRightCorner.Y + 1, true },
        { ScreenCorner.BottomRight, Testing.BottomRightCorner.X + 1, Testing.BottomRightCorner.Y - 1, false },
        { ScreenCorner.BottomRight, Testing.BottomRightCorner.X + 1, Testing.BottomRightCorner.Y + 0, true },
        { ScreenCorner.BottomRight, Testing.BottomRightCorner.X + 1, Testing.BottomRightCorner.Y + 1, true }
    };

    public static readonly TheoryData<ScreenCorner, int, int, bool> BottomLeftCornerTestCases = new()
    {
        // ScreenCorner            Mouse Cursor Position (X)       Mouse Cursor Position (Y)       Expected
        { ScreenCorner.BottomLeft, Testing.BottomLeftCorner.X - 1, Testing.BottomLeftCorner.Y - 1, false },
        { ScreenCorner.BottomLeft, Testing.BottomLeftCorner.X - 1, Testing.BottomLeftCorner.Y + 0, true },
        { ScreenCorner.BottomLeft, Testing.BottomLeftCorner.X - 1, Testing.BottomLeftCorner.Y + 1, true },
        { ScreenCorner.BottomLeft, Testing.BottomLeftCorner.X + 0, Testing.BottomLeftCorner.Y - 1, false },
        { ScreenCorner.BottomLeft, Testing.BottomLeftCorner.X + 0, Testing.BottomLeftCorner.Y + 0, true },
        { ScreenCorner.BottomLeft, Testing.BottomLeftCorner.X + 0, Testing.BottomLeftCorner.Y + 1, true },
        { ScreenCorner.BottomLeft, Testing.BottomLeftCorner.X + 1, Testing.BottomLeftCorner.Y - 1, false },
        { ScreenCorner.BottomLeft, Testing.BottomLeftCorner.X + 1, Testing.BottomLeftCorner.Y + 0, false },
        { ScreenCorner.BottomLeft, Testing.BottomLeftCorner.X + 1, Testing.BottomLeftCorner.Y + 1, false }
    };

    private readonly IScreenService _screenResolutionProvider;

    public ScreenServiceTests()
    {
        var provider = new ServiceProviderBuilder().Build();

        this._screenResolutionProvider = provider.GetRequiredService<IScreenService>();
    }

    [Theory]
    [MemberData(nameof(TopLeftCornerTestCases))]
    [MemberData(nameof(TopRightCornerTestCases))]
    [MemberData(nameof(BottomRightCornerTestCases))]
    [MemberData(nameof(BottomLeftCornerTestCases))]
    public void GetsTheCurrentCornerThresholdsForTheCurrentScreenResolution(ScreenCorner screenCorner, int x, int y, bool expectedResult)
    {
        // Act
        var result = this._screenResolutionProvider.IsMouseCursorInCorner(screenCorner, x, y);

        // Assert
        result.Should().Be(expectedResult);
    }
}
