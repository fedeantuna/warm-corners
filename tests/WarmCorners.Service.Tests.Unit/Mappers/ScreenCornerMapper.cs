using FluentAssertions;
using WarmCorners.Domain.Enums;
using WarmCorners.Service.Mappers;

namespace WarmCorners.Service.Tests.Unit.Mappers;

public class ScreenCornerMapper
{
    [Fact]
    public void ToScreenCorner_MapsScreenCornerFromStringToEnumIgnoringCase()
    {
        // Arrange
        const string screenCorner = "topLeft";

        // Act
        var result = screenCorner.ToScreenCorner();

        // Assert
        result.Should().Be(ScreenCorner.TopLeft);
    }

    [Fact]
    public void ToScreenCorner_MapsScreenCornerFromStringToNullWhenScreenCornerIsNotValid()
    {
        // Arrange
        const string screenCorner = "invalidScreenCorner";

        // Act
        var result = screenCorner.ToScreenCorner();

        // Assert
        result.Should().BeNull();
    }
}
