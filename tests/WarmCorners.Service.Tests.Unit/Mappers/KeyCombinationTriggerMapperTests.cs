using FluentAssertions;
using SharpHook.Native;
using WarmCorners.Core.Common;
using WarmCorners.Service.Configurations;
using WarmCorners.Service.Mappers;

namespace WarmCorners.Service.Tests.Unit.Mappers;

public class KeyCombinationTriggerMapperTests
{
    [Fact]
    public void ToKeyCombinationTriggers_MapsConfigurationToKeyCombinationTriggersIgnoringCase()
    {
        // Arrange
        const string topLeftCornerKeyCombination = "leftMeta+tab";
        const string topRightCornerKeyCombination = "rightControl+RIGHTAlt+deleTE";
        var keyCombinationTriggerConfigurations = new List<KeyCombinationTriggerConfiguration>
        {
            new()
            {
                ScreenCorner = "topLEFT",
                KeyCombination = topLeftCornerKeyCombination
            },
            new()
            {
                ScreenCorner = "topRight",
                KeyCombination = topRightCornerKeyCombination
            }
        };

        // Act
        var result = keyCombinationTriggerConfigurations.ToKeyCombinationTriggers().ToList();

        // Assert
        result.Should().HaveCount(keyCombinationTriggerConfigurations.Count);
        result.Should().Contain(kct => kct.ScreenCorner == ScreenCorner.TopLeft);
        result.Single(kct => kct.ScreenCorner == ScreenCorner.TopLeft).KeyCodes.Should()
            .ContainInOrder(KeyCode.VcLeftMeta, KeyCode.VcTab);
        result.Should().Contain(kct => kct.ScreenCorner == ScreenCorner.TopRight);
        result.Single(kct => kct.ScreenCorner == ScreenCorner.TopRight).KeyCodes.Should()
            .ContainInOrder(KeyCode.VcRightControl, KeyCode.VcRightAlt, KeyCode.VcDelete);
    }

    [Fact]
    public void ToKeyCombinationTriggers_MapsConfigurationToKeyCombinationTriggersIgnoringInvalidScreenCorners()
    {
        // Arrange
        const string centerCenterCornerKeyCombination = "leftMeta+tab";
        const string topRightCornerKeyCombination = "rightControl+RIGHTAlt+deleTE";
        var keyCombinationTriggerConfigurations = new List<KeyCombinationTriggerConfiguration>
        {
            new()
            {
                ScreenCorner = "centerCenter",
                KeyCombination = centerCenterCornerKeyCombination
            },
            new()
            {
                ScreenCorner = "topRight",
                KeyCombination = topRightCornerKeyCombination
            }
        };

        // Act
        var result = keyCombinationTriggerConfigurations.ToKeyCombinationTriggers().ToList();

        // Assert
        result.Should().ContainSingle(kct => kct.ScreenCorner == ScreenCorner.TopRight);
        result.Single(kct => kct.ScreenCorner == ScreenCorner.TopRight).KeyCodes.Should()
            .ContainInOrder(KeyCode.VcRightControl, KeyCode.VcRightAlt, KeyCode.VcDelete);
    }

    [Fact]
    public void ToKeyCombinationTriggers_MapsConfigurationToKeyCombinationTriggersIgnoringInvalidKeyCombinations()
    {
        // Arrange
        const string topLeftCornerKeyCombination = "leftInvalid+tab";
        const string topRightCornerKeyCombination = "rightControl+RIGHTAlt+deleTE";
        var keyCombinationTriggerConfigurations = new List<KeyCombinationTriggerConfiguration>
        {
            new()
            {
                ScreenCorner = "topLeft",
                KeyCombination = topLeftCornerKeyCombination
            },
            new()
            {
                ScreenCorner = "topRight",
                KeyCombination = topRightCornerKeyCombination
            }
        };

        // Act
        var result = keyCombinationTriggerConfigurations.ToKeyCombinationTriggers().ToList();

        // Assert
        result.Should().ContainSingle(kct => kct.ScreenCorner == ScreenCorner.TopRight);
        result.Single(kct => kct.ScreenCorner == ScreenCorner.TopRight).KeyCodes.Should()
            .ContainInOrder(KeyCode.VcRightControl, KeyCode.VcRightAlt, KeyCode.VcDelete);
    }
}
