using NSubstitute;
using Settlement.EventManagement.Models;
using Settlement.EventManagement.Services;
using Settlement.Internal.Constants;

namespace Settlement.EventManagement.UnitTests.Services;

public class ScoreTennisServiceTests
{
    private readonly ScoreTennisService service;
    private readonly ScoreSetService scoreSetService;

    public ScoreTennisServiceTests()
    {
        scoreSetService = Substitute.For<ScoreSetService>();
        service = new ScoreTennisServiceImp(scoreSetService);
    }

    [Fact]
    public void GetEventStatusWhenTotalSetIsRefundShouldReturnRefund()
    {
        // Arrange
        var resultDetails = new List<ResultDetail>
        {
            new()
            {
                ResourceId = ResourceType.Final,
                HomeValue = SetStatus.Refund
            }
        };
        var currentStatus = "completed";

        // Act
        var result = service.GetEventStatus(resultDetails, currentStatus);

        // Assert
        Assert.Equal(BetStatus.Refund, result);
    }

    [Fact]
    public void GetEventStatusWhenTotalSetIsNotRefundShouldReturnCurrentStatus()
    {
        // Arrange
        var resultDetails = new List<ResultDetail>
        {
            new()
            {
                ResourceId = ResourceType.Final,
                HomeValue = "2"
            }
        };
        var currentStatus = "completed";

        // Act
        var result = service.GetEventStatus(resultDetails, currentStatus);

        // Assert
        Assert.Equal(currentStatus, result);
    }

    [Fact]
    public void GetEventStatusWhenNoTotalSetFoundShouldReturnCurrentStatus()
    {
        // Arrange
        var resultDetails = new List<ResultDetail>
        {
            new()
            {
                ResourceId = "01",
                HomeValue = SetStatus.Refund
            }
        };
        var currentStatus = "completed";

        // Act
        var result = service.GetEventStatus(resultDetails, currentStatus);

        // Assert
        Assert.Equal(currentStatus, result);
    }

    [Fact]
    public void GetEventStatusWhenTotalSetHomeValueIsEmptyShouldReturnCurrentStatus()
    {
        // Arrange
        var resultDetails = new List<ResultDetail>
        {
            new()
            {
                ResourceId = ResourceType.Final,
                HomeValue = ""
            }
        };
        var currentStatus = "completed";

        // Act
        var result = service.GetEventStatus(resultDetails, currentStatus);

        // Assert
        Assert.Equal(currentStatus, result);
    }

    [Fact]
    public void GetEventStatusWhenTotalSetHomeValueIsNullShouldReturnCurrentStatus()
    {
        // Arrange
        var resultDetails = new List<ResultDetail>
        {
            new()
            {
                ResourceId = ResourceType.Final,
                HomeValue = null
            }
        };
        var currentStatus = "completed";

        // Act
        var result = service.GetEventStatus(resultDetails, currentStatus);

        // Assert
        Assert.Equal(currentStatus, result);
    }

    [Fact]
    public void ScoreLoaderShouldInitializeMoreScoresWithDefaultValues()
    {
        // Arrange
        var resultDetails = new List<ResultDetail>();
        var isLeader = false;

        // Act
        var result = service.ScoreLoader(resultDetails, isLeader);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Sets);
        Assert.Equal(5, result.Sets.Length);
        Assert.All(result.Sets, set => Assert.Equal("-", set.Home));
        Assert.Equal("-", result.HalfTimeScore.Home);
        Assert.Equal("-", result.FinalScore.Home);
    }

    [Fact]
    public void ScoreLoaderWhenSetResourcesExistShouldPopulateSets()
    {
        // Arrange
        var resultDetails = new List<ResultDetail>
        {
            new() { ResourceId = "01", HomeValue = "6", AwayValue = "4" },
            new() { ResourceId = "02", HomeValue = "7", AwayValue = "5" }
        };
        var isLeader = true;

        var expectedScore1 = new Score("6", "4");
        var expectedScore2 = new Score("7", "5");

        scoreSetService.GetScore(Arg.Is<ResultDetail>(r => r.ResourceId == "01"), isLeader).Returns(expectedScore1);
        scoreSetService.GetScore(Arg.Is<ResultDetail>(r => r.ResourceId == "02"), isLeader).Returns(expectedScore2);

        // Act
        var result = service.ScoreLoader(resultDetails, isLeader);

        // Assert
        Assert.Equal(expectedScore1, result.Sets[0]);
        Assert.Equal(expectedScore2, result.Sets[1]);
        scoreSetService.Received(1).GetScore(Arg.Is<ResultDetail>(r => r.ResourceId == "01"), isLeader);
        scoreSetService.Received(1).GetScore(Arg.Is<ResultDetail>(r => r.ResourceId == "02"), isLeader);
    }

    [Fact]
    public void ScoreLoaderWhenTotalSetExistsAndMatchIdGreaterThanZeroShouldSetFinalAndHalfTimeScores()
    {
        // Arrange
        var resultDetails = new List<ResultDetail>
        {
            new() { ResourceId = ResourceType.Final, HomeValue = "2", AwayValue = "1", MatchId = 1 }
        };
        var isLeader = false;

        var expectedFinalScore = new Score("2", "1");
        var expectedHalfTimeScore = new Score("3", "2");

        scoreSetService.GetScore(Arg.Is<ResultDetail>(r => r.ResourceId == ResourceType.Final), isLeader).Returns(expectedFinalScore);
        scoreSetService.SumScore(Arg.Any<IReadOnlyCollection<ResultDetail>>()).Returns(expectedHalfTimeScore);

        // Act
        var result = service.ScoreLoader(resultDetails, isLeader);

        // Assert
        Assert.Equal(expectedFinalScore, result.FinalScore);
        Assert.Equal(expectedHalfTimeScore, result.HalfTimeScore);
        scoreSetService.Received(1).GetScore(Arg.Is<ResultDetail>(r => r.ResourceId == ResourceType.Final), isLeader);
    }

    [Fact]
    public void ScoreLoaderWhenTotalSetExistsButMatchIdIsZeroShouldNotSetFinalAndHalfTimeScores()
    {
        // Arrange
        var resultDetails = new List<ResultDetail>
        {
            new() { ResourceId = ResourceType.Final, HomeValue = "2", AwayValue = "1", MatchId = 0 }
        };
        var isLeader = false;

        // Act
        var result = service.ScoreLoader(resultDetails, isLeader);

        // Assert
        Assert.Equal("-", result.FinalScore.Home);
        Assert.Equal("-", result.HalfTimeScore.Home);
        scoreSetService.DidNotReceive().GetScore(Arg.Is<ResultDetail>(r => r.ResourceId == ResourceType.Final), Arg.Any<bool>());
    }

    [Fact]
    public void ScoreLoaderWhenTotalSetHomeValueIsRefundShouldSetHalfTimeScoreToRefund()
    {
        // Arrange
        var resultDetails = new List<ResultDetail>
        {
            new() { ResourceId = ResourceType.Final, HomeValue = SetStatus.Refund, AwayValue = "1", MatchId = 1 }
        };
        var isLeader = false;

        var expectedFinalScore = new Score(SetStatus.Refund, "1");

        scoreSetService.GetScore(Arg.Is<ResultDetail>(r => r.ResourceId == ResourceType.Final), isLeader).Returns(expectedFinalScore);

        // Act
        var result = service.ScoreLoader(resultDetails, isLeader);

        // Assert
        Assert.Equal(SetStatus.Refund, result.HalfTimeScore.Home);
        Assert.Equal(SetStatus.Refund, result.HalfTimeScore.Away);
        scoreSetService.DidNotReceive().SumScore(Arg.Any<IReadOnlyCollection<ResultDetail>>());
    }

    [Fact]
    public void ScoreLoaderWhenTotalSetHomeValueIsNotRefundShouldCallSumScore()
    {
        // Arrange
        var resultDetails = new List<ResultDetail>
        {
            new() { ResourceId = ResourceType.Final, HomeValue = "2", AwayValue = "1", MatchId = 1 },
            new() { ResourceId = "01", HomeValue = "6", AwayValue = "4" }
        };
        var isLeader = false;

        var expectedFinalScore = new Score("2", "1");
        var expectedHalfTimeScore = new Score("3", "2");

        scoreSetService.GetScore(Arg.Is<ResultDetail>(r => r.ResourceId == ResourceType.Final), isLeader).Returns(expectedFinalScore);
        scoreSetService.SumScore(Arg.Any<IReadOnlyCollection<ResultDetail>>()).Returns(expectedHalfTimeScore);

        // Act
        var result = service.ScoreLoader(resultDetails, isLeader);

        // Assert
        Assert.Equal(expectedHalfTimeScore, result.HalfTimeScore);
        scoreSetService.Received(1).SumScore(Arg.Any<IReadOnlyCollection<ResultDetail>>());
    }

    [Fact]
    public void ScoreLoaderWhenSetResourcesAreOrderedShouldProcessInCorrectOrder()
    {
        // Arrange
        var resultDetails = new List<ResultDetail>
        {
            new() { ResourceId = "02", HomeValue = "7", AwayValue = "5" },
            new() { ResourceId = "01", HomeValue = "6", AwayValue = "4" }
        };
        var isLeader = true;

        var expectedScore1 = new Score("6", "4");
        var expectedScore2 = new Score("7", "5");

        scoreSetService.GetScore(Arg.Is<ResultDetail>(r => r.ResourceId == "01"), isLeader).Returns(expectedScore1);
        scoreSetService.GetScore(Arg.Is<ResultDetail>(r => r.ResourceId == "02"), isLeader).Returns(expectedScore2);

        // Act
        var result = service.ScoreLoader(resultDetails, isLeader);

        // Assert
        Assert.Equal(expectedScore1, result.Sets[0]); // Should be ordered by ResourceId
        Assert.Equal(expectedScore2, result.Sets[1]);
    }

    [Fact]
    public void ScoreLoaderWhenNoSetResourcesExistShouldKeepDefaultValues()
    {
        // Arrange
        var resultDetails = new List<ResultDetail>
        {
            new() { ResourceId = "06", HomeValue = "6", AwayValue = "4" } // Not in setResource list
        };
        var isLeader = true;

        // Act
        var result = service.ScoreLoader(resultDetails, isLeader);

        // Assert
        Assert.All(result.Sets, set => Assert.Equal("-", set.Home));
        scoreSetService.DidNotReceive().GetScore(Arg.Any<ResultDetail>(), Arg.Any<bool>());
    }

    [Fact]
    public void ScoreLoaderWhenMultipleSetResourcesExistShouldProcessAll()
    {
        // Arrange
        var resultDetails = new List<ResultDetail>
        {
            new() { ResourceId = "01", HomeValue = "6", AwayValue = "4" },
            new() { ResourceId = "02", HomeValue = "7", AwayValue = "5" },
            new() { ResourceId = "03", HomeValue = "6", AwayValue = "3" },
            new() { ResourceId = "04", HomeValue = "7", AwayValue = "6" },
            new() { ResourceId = "05", HomeValue = "6", AwayValue = "4" }
        };
        var isLeader = true;

        var expectedScores = new[]
        {
            new Score("6", "4"),
            new Score("7", "5"),
            new Score("6", "3"),
            new Score("7", "6"),
            new Score("6", "4")
        };

        // Setup mock for each specific ResultDetail
        scoreSetService.GetScore(Arg.Is<ResultDetail>(r => r.ResourceId == "01"), isLeader).Returns(expectedScores[0]);
        scoreSetService.GetScore(Arg.Is<ResultDetail>(r => r.ResourceId == "02"), isLeader).Returns(expectedScores[1]);
        scoreSetService.GetScore(Arg.Is<ResultDetail>(r => r.ResourceId == "03"), isLeader).Returns(expectedScores[2]);
        scoreSetService.GetScore(Arg.Is<ResultDetail>(r => r.ResourceId == "04"), isLeader).Returns(expectedScores[3]);
        scoreSetService.GetScore(Arg.Is<ResultDetail>(r => r.ResourceId == "05"), isLeader).Returns(expectedScores[4]);

        // Act
        var result = service.ScoreLoader(resultDetails, isLeader);

        // Assert
        for (int i = 0; i < 5; i++)
        {
            Assert.Equal(expectedScores[i], result.Sets[i]);
        }
        scoreSetService.Received(5).GetScore(Arg.Any<ResultDetail>(), isLeader);
    }

    [Fact]
    public void ScoreLoaderWhenTotalSetExistsWithValidMatchIdShouldSetBothFinalAndHalfTimeScores()
    {
        // Arrange
        var resultDetails = new List<ResultDetail>
        {
            new() { ResourceId = ResourceType.Final, HomeValue = "3", AwayValue = "2", MatchId = 1 },
            new() { ResourceId = "01", HomeValue = "6", AwayValue = "4" },
            new() { ResourceId = "02", HomeValue = "7", AwayValue = "5" }
        };
        var isLeader = false;

        var expectedFinalScore = new Score("3", "2");
        var expectedHalfTimeScore = new Score("13", "9");

        scoreSetService.GetScore(Arg.Is<ResultDetail>(r => r.ResourceId == ResourceType.Final), isLeader).Returns(expectedFinalScore);
        scoreSetService.SumScore(Arg.Any<IReadOnlyCollection<ResultDetail>>()).Returns(expectedHalfTimeScore);

        // Act
        var result = service.ScoreLoader(resultDetails, isLeader);

        // Assert
        Assert.Equal(expectedFinalScore, result.FinalScore);
        Assert.Equal(expectedHalfTimeScore, result.HalfTimeScore);
        scoreSetService.Received(1).GetScore(Arg.Is<ResultDetail>(r => r.ResourceId == ResourceType.Final), isLeader);
        scoreSetService.Received(1).SumScore(Arg.Any<IReadOnlyCollection<ResultDetail>>());
    }
}
