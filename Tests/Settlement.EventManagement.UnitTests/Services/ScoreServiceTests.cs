using NSubstitute;
using Settlement.EventManagement.Models;
using Settlement.EventManagement.Services;
using Settlement.Internal.Constants;

namespace Settlement.EventManagement.UnitTests.Services;

public class ScoreServiceTests
{
    private readonly ScoreService service;
    private readonly ScoreSetService scoreSetService;

    public ScoreServiceTests()
    {
        scoreSetService = Substitute.For<ScoreSetService>();
        service = new ScoreServiceImp(scoreSetService);
    }

    [Fact]
    public void IsIntervalScoreWhenBasketballAndHasIntervalScoresShouldReturnTrue()
    {
        // Arrange
        var sportType = SportType.BasketBall;
        var resultDetails = new List<ResultDetail>
        {
            new() { ResourceId = "Q1", HomeValue = "25", AwayValue = "20" },
            new() { ResourceId = ResourceType.Halftime, HomeValue = "50", AwayValue = "45" },
            new() { ResourceId = ResourceType.Final, HomeValue = "100", AwayValue = "95" }
        };

        // Act
        var result = service.IsIntervalScore(sportType, resultDetails);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsIntervalScoreWhenBasketballAndNoIntervalScoresShouldReturnFalse()
    {
        // Arrange
        var sportType = SportType.BasketBall;
        var resultDetails = new List<ResultDetail>
        {
            new() { ResourceId = ResourceType.Halftime, HomeValue = "50", AwayValue = "45" },
            new() { ResourceId = ResourceType.Final, HomeValue = "100", AwayValue = "95" }
        };

        // Act
        var result = service.IsIntervalScore(sportType, resultDetails);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsIntervalScoreWhenNonBasketballAndHasNonFinalScoresShouldReturnTrue()
    {
        // Arrange
        var sportType = SportType.Soccer;
        var resultDetails = new List<ResultDetail>
        {
            new() { ResourceId = "01", HomeValue = "1", AwayValue = "0" },
            new() { ResourceId = ResourceType.Final, HomeValue = "2", AwayValue = "1" }
        };

        // Act
        var result = service.IsIntervalScore(sportType, resultDetails);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsIntervalScoreWhenNonBasketballAndOnlyFinalScoresShouldReturnFalse()
    {
        // Arrange
        var sportType = SportType.Soccer;
        var resultDetails = new List<ResultDetail>
        {
            new() { ResourceId = ResourceType.Final, HomeValue = "2", AwayValue = "1" }
        };

        // Act
        var result = service.IsIntervalScore(sportType, resultDetails);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ScoreOfSetsShouldReturnScoresForAllResources()
    {
        // Arrange
        var resultDetails = new List<ResultDetail>
        {
            new() { ResourceId = "01", HomeValue = "6", AwayValue = "4", MatchId = 1 },
            new() { ResourceId = "02", HomeValue = "7", AwayValue = "5", MatchId = 1 }
        };
        var resources = new List<string> { "01", "02" };
        var isLeader = true;

        var expectedScore1 = new Score("6", "4");
        var expectedScore2 = new Score("7", "5");

        scoreSetService.GetScore(Arg.Is<ResultDetail>(r => r.ResourceId == "01"), isLeader).Returns(expectedScore1);
        scoreSetService.GetScore(Arg.Is<ResultDetail>(r => r.ResourceId == "02"), isLeader).Returns(expectedScore2);

        // Act
        var result = service.ScoreOfSets(resultDetails, resources, isLeader);

        // Assert
        Assert.Equal(2, result.Length);
        Assert.Equal(expectedScore1, result[0]);
        Assert.Equal(expectedScore2, result[1]);
        scoreSetService.Received(1).GetScore(Arg.Is<ResultDetail>(r => r.ResourceId == "01"), isLeader);
        scoreSetService.Received(1).GetScore(Arg.Is<ResultDetail>(r => r.ResourceId == "02"), isLeader);
    }

    [Fact]
    public void ScoreOfSetsWhenResourceNotFoundShouldReturnDefaultScore()
    {
        // Arrange
        var resultDetails = new List<ResultDetail>
        {
            new() { ResourceId = "01", HomeValue = "6", AwayValue = "4", MatchId = 1 }
        };
        var resources = new List<string> { "01", "02" };
        var isLeader = true;

        var expectedScore = new Score("6", "4");
        scoreSetService.GetScore(Arg.Is<ResultDetail>(r => r.ResourceId == "01"), isLeader).Returns(expectedScore);

        // Act
        var result = service.ScoreOfSets(resultDetails, resources, isLeader);

        // Assert
        Assert.Equal(2, result.Length);
        Assert.Equal(expectedScore, result[0]);
        Assert.Equal("-", result[1].Home);
        Assert.Equal("-", result[1].Away);
    }

    [Fact]
    public void HalftimeScoreWhenTotalPointSportShouldCallTotalPoints()
    {
        // Arrange
        var sportType = SportType.BeachVolleyball;
        var resultDetails = new List<ResultDetail>
        {
            new() { ResourceId = "0101", HomeValue = "21", AwayValue = "19" },
            new() { ResourceId = "0102", HomeValue = "21", AwayValue = "17" }
        };
        var isLeader = true;

        var expectedScore = new Score("42", "36");
        scoreSetService.SumScore(Arg.Any<IReadOnlyCollection<ResultDetail>>()).Returns(expectedScore);

        // Act
        var result = service.HalftimeScore(sportType, resultDetails, isLeader);

        // Assert
        Assert.Equal(expectedScore, result);
        scoreSetService.Received(1).SumScore(Arg.Any<IReadOnlyCollection<ResultDetail>>());
    }

    [Fact]
    public void HalftimeScoreWhenNonTotalPointSportShouldCallScoreByResource()
    {
        // Arrange
        var sportType = SportType.Soccer;
        var resultDetails = new List<ResultDetail>
        {
            new() { ResourceId = ResourceType.Halftime, HomeValue = "1", AwayValue = "0", MatchId = 1 }
        };
        var isLeader = false;

        var expectedScore = new Score("1", "0");
        scoreSetService.GetScore(Arg.Is<ResultDetail>(r => r.ResourceId == ResourceType.Halftime), isLeader).Returns(expectedScore);

        // Act
        var result = service.HalftimeScore(sportType, resultDetails, isLeader);

        // Assert
        Assert.Equal(expectedScore, result);
        scoreSetService.Received(1).GetScore(Arg.Is<ResultDetail>(r => r.ResourceId == ResourceType.Halftime), isLeader);
    }

    [Fact]
    public void HalftimeScoreWhenSupportSettleDetermineShouldClearScore()
    {
        // Arrange
        var sportType = SportType.BasketBall;
        var resultDetails = new List<ResultDetail>
        {
            new() { ResourceId = "Q1", HomeValue = "25", AwayValue = "20", SettlementStatusId = SettlementStatus.Abandoned },
            new() { ResourceId = "Q2", HomeValue = "25", AwayValue = "25", SettlementStatusId = SettlementStatus.Abandoned },
            new() { ResourceId = ResourceType.Halftime, HomeValue = "50", AwayValue = "45", MatchId = 1 }
        };
        var isLeader = true;

        var expectedScore = new Score("-", "-");
        scoreSetService.GetScore(Arg.Is<ResultDetail>(r => r.ResourceId == ResourceType.Halftime), isLeader).Returns(expectedScore);

        // Act
        var result = service.HalftimeScore(sportType, resultDetails, isLeader);

        // Assert
        Assert.Equal(expectedScore, result);
        // The ClearScore method should have cleared the Halftime result values
        // HomeValue getter returns null when value is empty due to UppercaseFirst method
        Assert.Null(resultDetails[2].HomeValue);
        Assert.Equal("", resultDetails[2].AwayValue);
    }

    [Fact]
    public void FinalScoreShouldCallScoreByResource()
    {
        // Arrange
        var sportType = SportType.Soccer;
        var resultDetails = new List<ResultDetail>
        {
            new() { ResourceId = ResourceType.Final, HomeValue = "2", AwayValue = "1", MatchId = 1 }
        };
        var isLeader = false;

        var expectedScore = new Score("2", "1");
        scoreSetService.GetScore(Arg.Is<ResultDetail>(r => r.ResourceId == ResourceType.Final), isLeader).Returns(expectedScore);

        // Act
        var result = service.FinalScore(sportType, resultDetails, isLeader);

        // Assert
        Assert.Equal(expectedScore, result);
        scoreSetService.Received(1).GetScore(Arg.Is<ResultDetail>(r => r.ResourceId == ResourceType.Final), isLeader);
    }

    [Fact]
    public void FinalScoreWhenSupportSettleDetermineShouldClearScore()
    {
        // Arrange
        var sportType = SportType.BasketBall;
        var resultDetails = new List<ResultDetail>
        {
            new() { ResourceId = "Q3", HomeValue = "25", AwayValue = "20", SettlementStatusId = SettlementStatus.Abandoned },
            new() { ResourceId = "Q4", HomeValue = "25", AwayValue = "25", SettlementStatusId = SettlementStatus.Abandoned },
            new() { ResourceId = ResourceType.Final, HomeValue = "100", AwayValue = "95", MatchId = 1 }
        };
        var isLeader = true;

        var expectedScore = new Score("-", "-");
        scoreSetService.GetScore(Arg.Is<ResultDetail>(r => r.ResourceId == ResourceType.Final), isLeader).Returns(expectedScore);

        // Act
        var result = service.FinalScore(sportType, resultDetails, isLeader);

        // Assert
        Assert.Equal(expectedScore, result);
        // The ClearScore method should have cleared the Final result values
        // HomeValue getter returns null when value is empty due to UppercaseFirst method
        Assert.Null(resultDetails[2].HomeValue);
        Assert.Equal("", resultDetails[2].AwayValue);
    }

    [Fact]
    public void VerifyScoreMercyWhenBaseballAndHasOtherInfoShouldReturnFormattedString()
    {
        // Arrange
        var sportType = SportType.BaseBall;
        var resultDetails = new List<ResultDetail>
        {
            new() { ResourceId = "0101", OtherInfo = "Mercy Rule Applied" }
        };

        // Act
        var result = service.VerifyScoreMercy(sportType, resultDetails);

        // Assert
        Assert.Equal("0101:Mercy Rule Applied", result);
    }

    [Fact]
    public void VerifyScoreMercyWhenBaseballAndNoOtherInfoShouldReturnEmptyString()
    {
        // Arrange
        var sportType = SportType.BaseBall;
        var resultDetails = new List<ResultDetail>
        {
            new() { ResourceId = "0101", OtherInfo = "" }
        };

        // Act
        var result = service.VerifyScoreMercy(sportType, resultDetails);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void VerifyScoreMercyWhenNonBaseballShouldReturnEmptyString()
    {
        // Arrange
        var sportType = SportType.Soccer;
        var resultDetails = new List<ResultDetail>
        {
            new() { ResourceId = "01", OtherInfo = "Some Info" }
        };

        // Act
        var result = service.VerifyScoreMercy(sportType, resultDetails);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void VerifyScoreMercyWhenBaseballAndNullOtherInfoShouldReturnEmptyString()
    {
        // Arrange
        var sportType = SportType.BaseBall;
        var resultDetails = new List<ResultDetail>
        {
            new() { ResourceId = "0101", OtherInfo = null }
        };

        // Act
        var result = service.VerifyScoreMercy(sportType, resultDetails);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Theory]
    [InlineData(SportType.BeachVolleyball)]
    [InlineData(SportType.Badminton)]
    [InlineData(SportType.VolleyBall)]
    public void HalftimeScoreWhenTotalPointSportsShouldCallTotalPoints(int sportType)
    {
        // Arrange
        var resultDetails = new List<ResultDetail>
        {
            new() { ResourceId = "0101", HomeValue = "21", AwayValue = "19" }
        };
        var isLeader = true;

        var expectedScore = new Score("21", "19");
        scoreSetService.SumScore(Arg.Any<IReadOnlyCollection<ResultDetail>>()).Returns(expectedScore);

        // Act
        var result = service.HalftimeScore(sportType, resultDetails, isLeader);

        // Assert
        Assert.Equal(expectedScore, result);
        scoreSetService.Received(1).SumScore(Arg.Any<IReadOnlyCollection<ResultDetail>>());
    }

    [Fact]
    public void HalftimeScoreWhenBeachVolleyballAndRefundMatchShouldReturnRefundScore()
    {
        // Arrange
        var sportType = SportType.BeachVolleyball;
        var resultDetails = new List<ResultDetail>
        {
            new() { ResourceId = ResourceType.Final, SettlementStatusId = SettlementStatus.Refund }
        };
        var isLeader = true;

        // Act
        var result = service.HalftimeScore(sportType, resultDetails, isLeader);

        // Assert
        Assert.Equal(SetStatus.Refund, result.Home);
        Assert.Equal(SetStatus.Refund, result.Away);
        scoreSetService.DidNotReceive().SumScore(Arg.Any<IReadOnlyCollection<ResultDetail>>());
    }

    [Fact]
    public void HalftimeScoreWhenNonBeachVolleyballAndRefundMatchShouldReturnRefundScore()
    {
        // Arrange
        var sportType = SportType.Badminton;
        var resultDetails = new List<ResultDetail>
        {
            new() { ResourceId = ResourceType.Final, SettlementStatusId = SettlementStatus.Refund }
        };
        var isLeader = true;

        // Act
        var result = service.HalftimeScore(sportType, resultDetails, isLeader);

        // Assert
        Assert.Equal(SetStatus.Refund, result.Home);
        Assert.Equal(SetStatus.Refund, result.Away);
        scoreSetService.DidNotReceive().SumScore(Arg.Any<IReadOnlyCollection<ResultDetail>>());
    }

    [Fact]
    public void HalftimeScoreWhenBeachVolleyballAndValidGamesShouldFilterCorrectly()
    {
        // Arrange
        var sportType = SportType.BeachVolleyball;
        var resultDetails = new List<ResultDetail>
        {
            new() { ResourceId = "0101", HomeValue = "21", AwayValue = "19", ResultStatusId = ResultStatus.AutoSettled },
            new() { ResourceId = "0102", HomeValue = "21", AwayValue = "17", ResultStatusId = ResultStatus.ManuallySettled },
            new() { ResourceId = "0103", HomeValue = "", AwayValue = "21" }, // Invalid - empty home value
            new() { ResourceId = "0104", HomeValue = "21", AwayValue = "", ResultStatusId = ResultStatus.AutoSettled }, // Invalid - empty away value
            new() { ResourceId = "0105", HomeValue = "21", AwayValue = "19", SettlementStatusId = SettlementStatus.Refund } // Invalid - refund
        };
        var isLeader = true;

        var expectedScore = new Score("42", "36");
        scoreSetService.SumScore(Arg.Any<IReadOnlyCollection<ResultDetail>>()).Returns(expectedScore);

        // Act
        var result = service.HalftimeScore(sportType, resultDetails, isLeader);

        // Assert
        Assert.Equal(expectedScore, result);
        scoreSetService.Received(1).SumScore(Arg.Is<IReadOnlyCollection<ResultDetail>>(r => r.Count() == 2));
    }

    [Fact]
    public void HalftimeScoreWhenNonBeachVolleyballAndValidGamesShouldFilterCorrectly()
    {
        // Arrange
        var sportType = SportType.Badminton;
        var resultDetails = new List<ResultDetail>
        {
            new() { ResourceId = "0101", HomeValue = "21", AwayValue = "19", ResultStatusId = ResultStatus.AutoSettled },
            new() { ResourceId = "0102", HomeValue = "21", AwayValue = "17", ResultStatusId = ResultStatus.ManuallySettled },
            new() { ResourceId = "0103", HomeValue = "21", AwayValue = "19", ResultStatusId = ResultStatus.Live } // Invalid - not settled
        };
        var isLeader = true;

        var expectedScore = new Score("42", "36");
        scoreSetService.SumScore(Arg.Any<IReadOnlyCollection<ResultDetail>>()).Returns(expectedScore);

        // Act
        var result = service.HalftimeScore(sportType, resultDetails, isLeader);

        // Assert
        Assert.Equal(expectedScore, result);
        scoreSetService.Received(1).SumScore(Arg.Is<IReadOnlyCollection<ResultDetail>>(r => r.Count() == 2));
    }

    [Fact]
    public void ScoreByResourceWhenResultDetailExistsAndMatchIdGreaterThanZeroShouldReturnScore()
    {
        // Arrange
        var resultDetails = new List<ResultDetail>
        {
            new() { ResourceId = "01", HomeValue = "6", AwayValue = "4", MatchId = 1, ResultStatusId = ResultStatus.AutoSettled }
        };
        var resource = "01";
        var isLeader = true;

        var expectedScore = new Score("6", "4");
        scoreSetService.GetScore(Arg.Any<ResultDetail>(), isLeader).Returns(expectedScore);

        // Act
        var result = service.ScoreOfSets(resultDetails, new List<string> { resource }, isLeader);

        // Assert
        Assert.Equal(expectedScore, result[0]);
        Assert.True(result[0].IsSettled);
    }

    [Fact]
    public void ScoreByResourceWhenResultDetailExistsButMatchIdIsZeroShouldReturnDefaultScore()
    {
        // Arrange
        var resultDetails = new List<ResultDetail>
        {
            new() { ResourceId = "01", HomeValue = "6", AwayValue = "4", MatchId = 0 }
        };
        var resource = "01";
        var isLeader = true;

        // Act
        var result = service.ScoreOfSets(resultDetails, new List<string> { resource }, isLeader);

        // Assert
        Assert.Equal("-", result[0].Home);
        Assert.Equal("-", result[0].Away);
        scoreSetService.DidNotReceive().GetScore(Arg.Any<ResultDetail>(), Arg.Any<bool>());
    }

    [Fact]
    public void ScoreByResourceWhenResultDetailNotExistsShouldReturnDefaultScore()
    {
        // Arrange
        var resultDetails = new List<ResultDetail>
        {
            new() { ResourceId = "02", HomeValue = "6", AwayValue = "4", MatchId = 1 }
        };
        var resource = "01";
        var isLeader = true;

        // Act
        var result = service.ScoreOfSets(resultDetails, new List<string> { resource }, isLeader);

        // Assert
        Assert.Equal("-", result[0].Home);
        Assert.Equal("-", result[0].Away);
        scoreSetService.DidNotReceive().GetScore(Arg.Any<ResultDetail>(), Arg.Any<bool>());
    }
}
