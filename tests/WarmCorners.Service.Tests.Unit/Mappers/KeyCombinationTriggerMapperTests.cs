using FluentAssertions;
using SharpHook.Native;
using WarmCorners.Core.Common;
using WarmCorners.Core.Services.Abstractions;
using WarmCorners.Service.Configurations;
using WarmCorners.Service.Mappers;

namespace WarmCorners.Service.Tests.Unit.Mappers;

public class KeyCombinationTriggerMapperTests
{
    [Fact]
    public void ToKeyCombinationTriggers_MapsConfigurationToKeyCombinationTriggersIgnoringCase()
    {
        // Arrange
        var keyCombinationTriggerConfigurations = new List<KeyCombinationTriggerConfiguration>
        {
            new()
            {
                ScreenCorner = "topLEFT",
                KeyCombination = Testing.SomeKeyCombination
            },
            new()
            {
                ScreenCorner = "topRight",
                KeyCombination = Testing.SomeOtherKeyCombination
            }
        };

        // Act
        var result = keyCombinationTriggerConfigurations.ToKeyCombinationTriggers().ToList();

        // Assert
        result.Should().HaveCount(keyCombinationTriggerConfigurations.Count);
        VerifyKeyCombinationOnTopLeftScreenCorner(result);
        VerifyKeyCombinationOnTopRightScreenCorner(result);
    }

    [Fact]
    public void ToKeyCombinationTriggers_MapsConfigurationToKeyCombinationTriggersIgnoringInvalidScreenCorners()
    {
        // Arrange
        var keyCombinationTriggerConfigurations = new List<KeyCombinationTriggerConfiguration>
        {
            new()
            {
                ScreenCorner = "centerCenter",
                KeyCombination = Testing.SomeKeyCombination
            },
            new()
            {
                ScreenCorner = "TopRight",
                KeyCombination = Testing.SomeOtherKeyCombination
            }
        };

        // Act
        var result = keyCombinationTriggerConfigurations.ToKeyCombinationTriggers().ToList();

        // Assert
        result.Should().HaveCount(1);
        VerifyKeyCombinationOnTopRightScreenCorner(result);
    }

    [Fact]
    public void ToKeyCombinationTriggers_MapsConfigurationToKeyCombinationTriggersIgnoringInvalidKeyCombinations()
    {
        // Arrange
        var keyCombinationTriggerConfigurations = new List<KeyCombinationTriggerConfiguration>
        {
            new()
            {
                ScreenCorner = "topLefT",
                KeyCombination = Testing.SomeInvalidKeyCombination
            },
            new()
            {
                ScreenCorner = "topRIGHT",
                KeyCombination = Testing.SomeOtherKeyCombination
            }
        };

        // Act
        var result = keyCombinationTriggerConfigurations.ToKeyCombinationTriggers().ToList();

        // Assert
        result.Should().HaveCount(1);
        VerifyKeyCombinationOnTopRightScreenCorner(result);
    }

    private static void VerifyKeyCombinationOnTopLeftScreenCorner(IReadOnlyCollection<KeyCombinationTrigger> result)
    {
        result.Should().Contain(kct => kct.ScreenCorner == ScreenCorner.TopLeft);
        result.Single(kct => kct.ScreenCorner == ScreenCorner.TopLeft).KeyCodes.Should()
            .ContainInOrder(KeyCode.VcLeftMeta, KeyCode.VcTab);
    }
    
    private static void VerifyKeyCombinationOnTopRightScreenCorner(IReadOnlyCollection<KeyCombinationTrigger> result)
    {
        result.Should().ContainSingle(kct => kct.ScreenCorner == ScreenCorner.TopRight);
        result.Single(kct => kct.ScreenCorner == ScreenCorner.TopRight).KeyCodes.Should()
            .ContainInOrder(KeyCode.VcRightControl, KeyCode.VcRightAlt, KeyCode.VcDelete);
    }
}
