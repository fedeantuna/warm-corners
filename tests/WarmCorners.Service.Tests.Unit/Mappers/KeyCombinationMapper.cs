using FluentAssertions;
using SharpHook.Native;
using WarmCorners.Service.Mappers;

namespace WarmCorners.Service.Tests.Unit.Mappers;

public class KeyCombinationMapper
{
    [Fact]
    public void ToKeyCombinationKeyCodes_MapsKeyCombinationFromStringToKeyCodesIgnoringCase()
    {
        // Arrange
        const string keyCombination = "leftMeta+TAB";

        // Act
        var result = keyCombination.ToKeyCombinationKeyCodes();

        // Assert
        result.Should().HaveCount(2);
        result.Should().ContainInOrder(KeyCode.VcLeftMeta, KeyCode.VcTab);
    }

    [Fact]
    public void ToKeyCombinationKeyCodes_MapsKeyCombinationFromStringToEmptyListWhenKeyCombinationIsNotValid()
    {
        // Arrange
        const string keyCombination = "leftMeta+invalidKey";

        // Act
        var result = keyCombination.ToKeyCombinationKeyCodes();

        // Assert
        result.Should().BeEmpty();
    }
}
