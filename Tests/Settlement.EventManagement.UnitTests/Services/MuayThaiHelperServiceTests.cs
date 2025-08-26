using Settlement.EventManagement.Services;

namespace Settlement.EventManagement.UnitTests.Services;

public class MuayThaiHelperServiceTests
{
    private readonly MuayThaiHelperService service;

    public MuayThaiHelperServiceTests()
    {
        service = new MuayThaiHelperServiceImp();
    }

    [Theory]
    [InlineData(1, 0, "1", "0", "1")]
    [InlineData(2, 0, "1", "0", "2")]
    [InlineData(3, 0, "1", "0", "3")]
    [InlineData(4, 0, "1", "0", "4")]
    [InlineData(5, 0, "1", "0", "5")]
    public void MapSboToSettleWhenHomeValueBetween1And5AndAwayValueIs0ShouldMapTo1And0WithRound(int homeValue, int awayValue, string expectedHome, string expectedAway, string expectedRound)
    {
        // Act
        var result = service.MapSboToSettle(homeValue, awayValue);

        // Assert
        Assert.Equal(expectedHome, result.Home);
        Assert.Equal(expectedAway, result.Away);
        Assert.Equal(expectedRound, result.Round);
    }

    [Theory]
    [InlineData(0, 1, "0", "1", "1")]
    [InlineData(0, 2, "0", "1", "2")]
    [InlineData(0, 3, "0", "1", "3")]
    [InlineData(0, 4, "0", "1", "4")]
    [InlineData(0, 5, "0", "1", "5")]
    public void MapSboToSettleWhenHomeValueIs0AndAwayValueBetween1And5ShouldMapTo0And1WithRound(int homeValue, int awayValue, string expectedHome, string expectedAway, string expectedRound)
    {
        // Act
        var result = service.MapSboToSettle(homeValue, awayValue);

        // Assert
        Assert.Equal(expectedHome, result.Home);
        Assert.Equal(expectedAway, result.Away);
        Assert.Equal(expectedRound, result.Round);
    }

    [Fact]
    public void MapSboToSettleWhenHomeValueIs6AndAwayValueIs0ShouldMapTo5And0WithEmptyRound()
    {
        // Arrange
        var homeValue = 6;
        var awayValue = 0;
        var expectedHome = "5";
        var expectedAway = "0";
        var expectedRound = string.Empty;

        // Act
        var result = service.MapSboToSettle(homeValue, awayValue);

        // Assert
        Assert.Equal(expectedHome, result.Home);
        Assert.Equal(expectedAway, result.Away);
        Assert.Equal(expectedRound, result.Round);
    }

    [Fact]
    public void MapSboToSettleWhenHomeValueIs0AndAwayValueIs6ShouldMapTo0And5WithEmptyRound()
    {
        // Arrange
        var homeValue = 0;
        var awayValue = 6;
        var expectedHome = "0";
        var expectedAway = "5";
        var expectedRound = string.Empty;

        // Act
        var result = service.MapSboToSettle(homeValue, awayValue);

        // Assert
        Assert.Equal(expectedHome, result.Home);
        Assert.Equal(expectedAway, result.Away);
        Assert.Equal(expectedRound, result.Round);
    }

    [Fact]
    public void MapSboToSettleWhenHomeValueIs6AndAwayValueIs6ShouldMapTo5And5WithEmptyRound()
    {
        // Arrange
        var homeValue = 6;
        var awayValue = 6;
        var expectedHome = "5";
        var expectedAway = "5";
        var expectedRound = string.Empty;

        // Act
        var result = service.MapSboToSettle(homeValue, awayValue);

        // Assert
        Assert.Equal(expectedHome, result.Home);
        Assert.Equal(expectedAway, result.Away);
        Assert.Equal(expectedRound, result.Round);
    }

    [Theory]
    [InlineData(0, 0, "0", "0", "")]
    [InlineData(7, 0, "7", "0", "")]
    [InlineData(0, 7, "0", "7", "")]
    [InlineData(10, 5, "10", "5", "")]
    [InlineData(3, 4, "3", "4", "")]
    [InlineData(1, 1, "1", "1", "")]
    [InlineData(2, 3, "2", "3", "")]
    [InlineData(8, 9, "8", "9", "")]
    [InlineData(15, 20, "15", "20", "")]
    public void MapSboToSettleWhenValuesDoNotMatchSpecialCasesShouldReturnOriginalValuesAsStringsWithEmptyRound(int homeValue, int awayValue, string expectedHome, string expectedAway, string expectedRound)
    {
        // Act
        var result = service.MapSboToSettle(homeValue, awayValue);

        // Assert
        Assert.Equal(expectedHome, result.Home);
        Assert.Equal(expectedAway, result.Away);
        Assert.Equal(expectedRound, result.Round);
    }

    [Theory]
    [InlineData(1, 1, "1", "1", "")]
    [InlineData(2, 2, "2", "2", "")]
    [InlineData(3, 3, "3", "3", "")]
    [InlineData(4, 4, "4", "4", "")]
    [InlineData(5, 5, "5", "5", "")]
    public void MapSboToSettleWhenHomeValueBetween1And5AndAwayValueSameShouldReturnOriginalValuesWithEmptyRound(int homeValue, int awayValue, string expectedHome, string expectedAway, string expectedRound)
    {
        // Act
        var result = service.MapSboToSettle(homeValue, awayValue);

        // Assert
        Assert.Equal(expectedHome, result.Home);
        Assert.Equal(expectedAway, result.Away);
        Assert.Equal(expectedRound, result.Round);
    }

    [Theory]
    [InlineData(6, 1, "6", "1", "")]
    [InlineData(6, 2, "6", "2", "")]
    [InlineData(6, 3, "6", "3", "")]
    [InlineData(6, 4, "6", "4", "")]
    [InlineData(6, 5, "6", "5", "")]
    public void MapSboToSettleWhenHomeValueIs6AndAwayValueBetween1And5ShouldReturnOriginalValuesWithEmptyRound(int homeValue, int awayValue, string expectedHome, string expectedAway, string expectedRound)
    {
        // Act
        var result = service.MapSboToSettle(homeValue, awayValue);

        // Assert
        Assert.Equal(expectedHome, result.Home);
        Assert.Equal(expectedAway, result.Away);
        Assert.Equal(expectedRound, result.Round);
    }

    [Theory]
    [InlineData(1, 6, "1", "6", "")]
    [InlineData(2, 6, "2", "6", "")]
    [InlineData(3, 6, "3", "6", "")]
    [InlineData(4, 6, "4", "6", "")]
    [InlineData(5, 6, "5", "6", "")]
    public void MapSboToSettleWhenHomeValueBetween1And5AndAwayValueIs6ShouldReturnOriginalValuesWithEmptyRound(int homeValue, int awayValue, string expectedHome, string expectedAway, string expectedRound)
    {
        // Act
        var result = service.MapSboToSettle(homeValue, awayValue);

        // Assert
        Assert.Equal(expectedHome, result.Home);
        Assert.Equal(expectedAway, result.Away);
        Assert.Equal(expectedRound, result.Round);
    }

    [Theory]
    [InlineData(-1, 0, "-1", "0", "")]
    [InlineData(0, -1, "0", "-1", "")]
    [InlineData(-5, -3, "-5", "-3", "")]
    public void MapSboToSettleWhenNegativeValuesShouldReturnOriginalValuesAsStringsWithEmptyRound(int homeValue, int awayValue, string expectedHome, string expectedAway, string expectedRound)
    {
        // Act
        var result = service.MapSboToSettle(homeValue, awayValue);

        // Assert
        Assert.Equal(expectedHome, result.Home);
        Assert.Equal(expectedAway, result.Away);
        Assert.Equal(expectedRound, result.Round);
    }

    [Theory]
    [InlineData(int.MaxValue, 0, "2147483647", "0", "")]
    [InlineData(0, int.MaxValue, "0", "2147483647", "")]
    [InlineData(int.MinValue, 0, "-2147483648", "0", "")]
    [InlineData(0, int.MinValue, "0", "-2147483648", "")]
    public void MapSboToSettleWhenExtremeValuesShouldReturnOriginalValuesAsStringsWithEmptyRound(int homeValue, int awayValue, string expectedHome, string expectedAway, string expectedRound)
    {
        // Act
        var result = service.MapSboToSettle(homeValue, awayValue);

        // Assert
        Assert.Equal(expectedHome, result.Home);
        Assert.Equal(expectedAway, result.Away);
        Assert.Equal(expectedRound, result.Round);
    }

    // Additional edge cases for better coverage
    [Theory]
    [InlineData(6, 7, "6", "7", "")]
    [InlineData(7, 6, "7", "6", "")]
    [InlineData(6, 8, "6", "8", "")]
    [InlineData(8, 6, "8", "6", "")]
    public void MapSboToSettleWhenOneValueIs6AndOtherIsGreaterThan6ShouldReturnOriginalValuesWithEmptyRound(int homeValue, int awayValue, string expectedHome, string expectedAway, string expectedRound)
    {
        // Act
        var result = service.MapSboToSettle(homeValue, awayValue);

        // Assert
        Assert.Equal(expectedHome, result.Home);
        Assert.Equal(expectedAway, result.Away);
        Assert.Equal(expectedRound, result.Round);
    }

    [Theory]
    [InlineData(0, 0, "0", "0", "")]
    [InlineData(100, 0, "100", "0", "")]
    [InlineData(0, 100, "0", "100", "")]
    [InlineData(999, 999, "999", "999", "")]
    public void MapSboToSettleWhenValuesAreOutsideSpecialRangesShouldReturnOriginalValuesWithEmptyRound(int homeValue, int awayValue, string expectedHome, string expectedAway, string expectedRound)
    {
        // Act
        var result = service.MapSboToSettle(homeValue, awayValue);

        // Assert
        Assert.Equal(expectedHome, result.Home);
        Assert.Equal(expectedAway, result.Away);
        Assert.Equal(expectedRound, result.Round);
    }

    [Fact]
    public void MapSboToSettleShouldReturnCorrectTupleStructure()
    {
        // Arrange
        var homeValue = 3;
        var awayValue = 0;

        // Act
        var result = service.MapSboToSettle(homeValue, awayValue);

        // Assert
        Assert.IsType<(string Home, string Away, string Round)>(result);
        Assert.Equal("1", result.Home);
        Assert.Equal("0", result.Away);
        Assert.Equal("3", result.Round);
    }

    [Theory]
    [InlineData(1, 0)]
    [InlineData(2, 0)]
    [InlineData(3, 0)]
    [InlineData(4, 0)]
    [InlineData(5, 0)]
    public void MapSboToSettleWhenHomeValueBetween1And5AndAwayValueIs0ShouldHaveRoundEqualToHomeValue(int homeValue, int awayValue)
    {
        // Act
        var result = service.MapSboToSettle(homeValue, awayValue);

        // Assert
        Assert.Equal(homeValue.ToString(), result.Round);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(0, 2)]
    [InlineData(0, 3)]
    [InlineData(0, 4)]
    [InlineData(0, 5)]
    public void MapSboToSettleWhenHomeValueIs0AndAwayValueBetween1And5ShouldHaveRoundEqualToAwayValue(int homeValue, int awayValue)
    {
        // Act
        var result = service.MapSboToSettle(homeValue, awayValue);

        // Assert
        Assert.Equal(awayValue.ToString(), result.Round);
    }
}
