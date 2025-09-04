using NSubstitute;
using Settlement.EventManagement.Models;
using Settlement.EventManagement.Services;
using Settlement.Internal.Constants;

namespace Settlement.EventManagement.UnitTests.Services;

public class ExternalScoreServiceTests
{
    private readonly ExternalScoreService service;
    private readonly MuayThaiHelperService muayThaiHelperService;

    public ExternalScoreServiceTests()
    {
        muayThaiHelperService = Substitute.For<MuayThaiHelperService>();
        service = new ExternalScoreServiceImp(muayThaiHelperService);
    }

    [Fact]
    public void IsInconsistentFullTimeScoreWhenFinalExternalResultNotFoundShouldReturnFalse()
    {
        // Arrange
        var matchId = 1;
        var userConfirmation = "-";
        var resultDetails = new List<ResultDetail>
        {
            new() { MatchId = 1, SourceId = SourceType.Normal, ResourceId = ResourceType.Final, HomeValue = "2", AwayValue = "1" }
        };

        // Act
        var result = service.IsInconsistentFullTimeScore(matchId, userConfirmation, resultDetails);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsInconsistentFullTimeScoreWhenCurrentResultNotFoundShouldReturnTrue()
    {
        // Arrange
        var matchId = 1;
        var userConfirmation = "-";
        var resultDetails = new List<ResultDetail>
        {
            new() { MatchId = 1, SourceId = SourceType.Sbo, ResourceId = ResourceType.Final, HomeValue = "2", AwayValue = "1" }
        };

        // Act
        var result = service.IsInconsistentFullTimeScore(matchId, userConfirmation, resultDetails);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsInconsistentFullTimeScoreWhenScoresAreEqualShouldReturnFalse()
    {
        // Arrange
        var matchId = 1;
        var userConfirmation = "-";
        var resultDetails = new List<ResultDetail>
        {
            new() { MatchId = 1, SourceId = SourceType.Sbo, ResourceId = ResourceType.Final, HomeValue = "2", AwayValue = "1" },
            new() { MatchId = 1, SourceId = SourceType.Normal, ResourceId = ResourceType.Final, HomeValue = "2", AwayValue = "1" }
        };

        // Act
        var result = service.IsInconsistentFullTimeScore(matchId, userConfirmation, resultDetails);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsInconsistentFullTimeScoreWhenScoresAreDifferentAndHasValidScoresShouldReturnTrue()
    {
        // Arrange
        var matchId = 1;
        var userConfirmation = "-";
        var resultDetails = new List<ResultDetail>
        {
            new() { MatchId = 1, SourceId = SourceType.Sbo, ResourceId = ResourceType.Final, HomeValue = "2", AwayValue = "1" },
            new() { MatchId = 1, SourceId = SourceType.Normal, ResourceId = ResourceType.Final, HomeValue = "3", AwayValue = "2" }
        };

        // Act
        var result = service.IsInconsistentFullTimeScore(matchId, userConfirmation, resultDetails);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsInconsistentFullTimeScoreWhenScoresAreDifferentAndNoValidScoresButHasUserConfirmationShouldReturnTrue()
    {
        // Arrange
        var matchId = 1;
        var userConfirmation = "confirmed";
        var resultDetails = new List<ResultDetail>
        {
            new() { MatchId = 1, SourceId = SourceType.Sbo, ResourceId = ResourceType.Final, HomeValue = "2", AwayValue = "1" },
            new() { MatchId = 1, SourceId = SourceType.Normal, ResourceId = ResourceType.Final, HomeValue = "", AwayValue = "" }
        };

        // Act
        var result = service.IsInconsistentFullTimeScore(matchId, userConfirmation, resultDetails);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsInconsistentFullTimeScoreWhenScoresAreDifferentAndNoValidScoresAndNoUserConfirmationShouldReturnFalse()
    {
        // Arrange
        var matchId = 1;
        var userConfirmation = "-";
        var resultDetails = new List<ResultDetail>
        {
            new() { MatchId = 1, SourceId = SourceType.Sbo, ResourceId = ResourceType.Final, HomeValue = "2", AwayValue = "1" },
            new() { MatchId = 1, SourceId = SourceType.Normal, ResourceId = ResourceType.Final, HomeValue = "", AwayValue = "" }
        };

        // Act
        var result = service.IsInconsistentFullTimeScore(matchId, userConfirmation, resultDetails);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsInconsistentFullTimeScoreWithMatchWhenFinalExternalResultNotFoundShouldReturnFalse()
    {
        // Arrange
        var match = CreateTestMatch();
        var resultDetails = new List<ResultDetail>
        {
            new() { MatchId = 1, SourceId = SourceType.Normal, ResourceId = ResourceType.Final, HomeValue = "2", AwayValue = "1" }
        };

        // Act
        var result = service.IsInconsistentFullTimeScore(match, resultDetails);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsInconsistentFullTimeScoreWithMatchWhenNoFinalScoresAndSuspendedShouldReturnTrue()
    {
        // Arrange
        var match = CreateTestMatch();
        match.FinalHomeScore = null;
        match.FinalAwayScore = null;
        match.SuspenderFullTime = "suspended";
        var resultDetails = new List<ResultDetail>
        {
            new() { MatchId = 1, SourceId = SourceType.Sbo, ResourceId = ResourceType.Final, HomeValue = "2", AwayValue = "1" }
        };

        // Act
        var result = service.IsInconsistentFullTimeScore(match, resultDetails);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsInconsistentFullTimeScoreWithMatchWhenNoFinalScoresAndNotSuspendedShouldReturnFalse()
    {
        // Arrange
        var match = CreateTestMatch();
        match.FinalHomeScore = null;
        match.FinalAwayScore = null;
        match.SuspenderFullTime = "-";
        var resultDetails = new List<ResultDetail>
        {
            new() { MatchId = 1, SourceId = SourceType.Sbo, ResourceId = ResourceType.Final, HomeValue = "2", AwayValue = "1" }
        };

        // Act
        var result = service.IsInconsistentFullTimeScore(match, resultDetails);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsInconsistentFullTimeScoreWithMatchWhenNonMuayThaiAndScoresAreEqualShouldReturnFalse()
    {
        // Arrange
        var match = CreateTestMatch();
        match.SportType = SportType.Soccer;
        match.FinalHomeScore = 2;
        match.FinalAwayScore = 1;
        var resultDetails = new List<ResultDetail>
        {
            new() { MatchId = 1, SourceId = SourceType.Sbo, ResourceId = ResourceType.Final, HomeValue = "2", AwayValue = "1" }
        };

        // Act
        var result = service.IsInconsistentFullTimeScore(match, resultDetails);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsInconsistentFullTimeScoreWithMatchWhenNonMuayThaiAndScoresAreDifferentShouldReturnTrue()
    {
        // Arrange
        var match = CreateTestMatch();
        match.SportType = SportType.Soccer;
        match.FinalHomeScore = 3;
        match.FinalAwayScore = 2;
        var resultDetails = new List<ResultDetail>
        {
            new() { MatchId = 1, SourceId = SourceType.Sbo, ResourceId = ResourceType.Final, HomeValue = "2", AwayValue = "1" }
        };

        // Act
        var result = service.IsInconsistentFullTimeScore(match, resultDetails);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsInconsistentFullTimeScoreWithMatchWhenMuayThaiShouldMapScores()
    {
        // Arrange
        var match = CreateTestMatch();
        match.SportType = SportType.MuayThai;
        match.FinalHomeScore = 1;
        match.FinalAwayScore = 0;
        match.BetStatus = "3";
        var resultDetails = new List<ResultDetail>
        {
            new() { MatchId = 1, SourceId = SourceType.Sbo, ResourceId = ResourceType.Final, HomeValue = "3", AwayValue = "0" }
        };

        muayThaiHelperService.MapSboToSettle(3, 0).Returns(("1", "0", "3"));

        // Act
        var result = service.IsInconsistentFullTimeScore(match, resultDetails);

        // Assert
        Assert.False(result);
        muayThaiHelperService.Received(1).MapSboToSettle(3, 0);
    }

    [Fact]
    public void IsInconsistentHalftimeScoreWhenHalftimeExternalResultNotFoundShouldReturnFalse()
    {
        // Arrange
        var match = CreateTestMatch();
        var resultDetails = new List<ResultDetail>
        {
            new() { MatchId = 1, SourceId = SourceType.Normal, ResourceId = ResourceType.Halftime, HomeValue = "1", AwayValue = "0" }
        };

        // Act
        var result = service.IsInconsistentHalftimeScore(match, resultDetails);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsInconsistentHalftimeScoreWhenNoHalftimeScoresAndSuspendedShouldReturnTrue()
    {
        // Arrange
        var match = CreateTestMatch();
        match.HalfTimeHomeScore = null;
        match.HalfTimeAwayScore = null;
        match.SuspenderHalfTime = "suspended";
        var resultDetails = new List<ResultDetail>
        {
            new() { MatchId = 1, SourceId = SourceType.Sbo, ResourceId = ResourceType.Halftime, HomeValue = "1", AwayValue = "0" }
        };

        // Act
        var result = service.IsInconsistentHalftimeScore(match, resultDetails);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsInconsistentHalftimeScoreWhenNoHalftimeScoresAndNotSuspendedShouldReturnFalse()
    {
        // Arrange
        var match = CreateTestMatch();
        match.HalfTimeHomeScore = null;
        match.HalfTimeAwayScore = null;
        match.SuspenderHalfTime = "-";
        var resultDetails = new List<ResultDetail>
        {
            new() { MatchId = 1, SourceId = SourceType.Sbo, ResourceId = ResourceType.Halftime, HomeValue = "1", AwayValue = "0" }
        };

        // Act
        var result = service.IsInconsistentHalftimeScore(match, resultDetails);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsInconsistentHalftimeScoreWhenScoresAreEqualShouldReturnFalse()
    {
        // Arrange
        var match = CreateTestMatch();
        match.HalfTimeHomeScore = 1;
        match.HalfTimeAwayScore = 0;
        var resultDetails = new List<ResultDetail>
        {
            new() { MatchId = 1, SourceId = SourceType.Sbo, ResourceId = ResourceType.Halftime, HomeValue = "1", AwayValue = "0" }
        };

        // Act
        var result = service.IsInconsistentHalftimeScore(match, resultDetails);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsInconsistentHalftimeScoreWhenScoresAreDifferentShouldReturnTrue()
    {
        // Arrange
        var match = CreateTestMatch();
        match.HalfTimeHomeScore = 2;
        match.HalfTimeAwayScore = 1;
        var resultDetails = new List<ResultDetail>
        {
            new() { MatchId = 1, SourceId = SourceType.Sbo, ResourceId = ResourceType.Halftime, HomeValue = "1", AwayValue = "0" }
        };

        // Act
        var result = service.IsInconsistentHalftimeScore(match, resultDetails);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsInconsistentHalftimeScoreWhenHomeScoreIsNullShouldReturnTrue()
    {
        // Arrange
        var match = CreateTestMatch();
        match.HalfTimeHomeScore = null;
        match.HalfTimeAwayScore = 0;
        var resultDetails = new List<ResultDetail>
        {
            new() { MatchId = 1, SourceId = SourceType.Sbo, ResourceId = ResourceType.Halftime, HomeValue = "1", AwayValue = "0" }
        };

        // Act
        var result = service.IsInconsistentHalftimeScore(match, resultDetails);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsInconsistentHalftimeScoreWhenAwayScoreIsNullShouldReturnTrue()
    {
        // Arrange
        var match = CreateTestMatch();
        match.HalfTimeHomeScore = 1;
        match.HalfTimeAwayScore = null;
        var resultDetails = new List<ResultDetail>
        {
            new() { MatchId = 1, SourceId = SourceType.Sbo, ResourceId = ResourceType.Halftime, HomeValue = "1", AwayValue = "0" }
        };

        // Act
        var result = service.IsInconsistentHalftimeScore(match, resultDetails);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsInconsistentFullTimeScoreWithMatchWhenHomeScoreIsNullShouldReturnTrue()
    {
        // Arrange
        var match = CreateTestMatch();
        match.FinalHomeScore = null;
        match.FinalAwayScore = 1;
        var resultDetails = new List<ResultDetail>
        {
            new() { MatchId = 1, SourceId = SourceType.Sbo, ResourceId = ResourceType.Final, HomeValue = "2", AwayValue = "1" }
        };

        // Act
        var result = service.IsInconsistentFullTimeScore(match, resultDetails);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsInconsistentFullTimeScoreWithMatchWhenAwayScoreIsNullShouldReturnTrue()
    {
        // Arrange
        var match = CreateTestMatch();
        match.FinalHomeScore = 2;
        match.FinalAwayScore = null;
        var resultDetails = new List<ResultDetail>
        {
            new() { MatchId = 1, SourceId = SourceType.Sbo, ResourceId = ResourceType.Final, HomeValue = "2", AwayValue = "1" }
        };

        // Act
        var result = service.IsInconsistentFullTimeScore(match, resultDetails);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsInconsistentFullTimeScoreWhenExternalResultHasInvalidScoresShouldBeFilteredOut()
    {
        // Arrange
        var matchId = 1;
        var userConfirmation = "-";
        var resultDetails = new List<ResultDetail>
        {
            new() { MatchId = 1, SourceId = SourceType.Sbo, ResourceId = ResourceType.Final, HomeValue = "", AwayValue = "1" },
            new() { MatchId = 1, SourceId = SourceType.Sbo, ResourceId = ResourceType.Final, HomeValue = "2", AwayValue = "" },
            new() { MatchId = 1, SourceId = SourceType.Sbo, ResourceId = ResourceType.Final, HomeValue = null, AwayValue = "1" },
            new() { MatchId = 1, SourceId = SourceType.Sbo, ResourceId = ResourceType.Final, HomeValue = "2", AwayValue = null }
        };

        // Act
        var result = service.IsInconsistentFullTimeScore(matchId, userConfirmation, resultDetails);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsInconsistentFullTimeScoreWhenMultipleExternalResultsShouldUseFinal()
    {
        // Arrange
        var matchId = 1;
        var userConfirmation = "-";
        var resultDetails = new List<ResultDetail>
        {
            new() { MatchId = 1, SourceId = SourceType.Sbo, ResourceId = "01", HomeValue = "1", AwayValue = "0" },
            new() { MatchId = 1, SourceId = SourceType.Sbo, ResourceId = ResourceType.Final, HomeValue = "2", AwayValue = "1" },
            new() { MatchId = 1, SourceId = SourceType.Normal, ResourceId = ResourceType.Final, HomeValue = "3", AwayValue = "2" }
        };

        // Act
        var result = service.IsInconsistentFullTimeScore(matchId, userConfirmation, resultDetails);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsInconsistentHalftimeScoreWhenMultipleExternalResultsShouldUseHalftime()
    {
        // Arrange
        var match = CreateTestMatch();
        match.HalfTimeHomeScore = 1;
        match.HalfTimeAwayScore = 0;
        var resultDetails = new List<ResultDetail>
        {
            new() { MatchId = 1, SourceId = SourceType.Sbo, ResourceId = "01", HomeValue = "2", AwayValue = "1" },
            new() { MatchId = 1, SourceId = SourceType.Sbo, ResourceId = ResourceType.Halftime, HomeValue = "2", AwayValue = "1" },
            new() { MatchId = 1, SourceId = SourceType.Sbo, ResourceId = ResourceType.Final, HomeValue = "3", AwayValue = "2" }
        };

        // Act
        var result = service.IsInconsistentHalftimeScore(match, resultDetails);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsInconsistentFullTimeScoreWhenMuayThaiAndInvalidScoresShouldNotCallHelper()
    {
        // Arrange
        var match = CreateTestMatch();
        match.SportType = SportType.MuayThai;
        match.FinalHomeScore = 1;
        match.FinalAwayScore = 0;
        var resultDetails = new List<ResultDetail>
        {
            new() { MatchId = 1, SourceId = SourceType.Sbo, ResourceId = ResourceType.Final, HomeValue = "invalid", AwayValue = "0" }
        };

        // Act
        var result = service.IsInconsistentFullTimeScore(match, resultDetails);

        // Assert
        Assert.True(result);
        muayThaiHelperService.DidNotReceive().MapSboToSettle(Arg.Any<int>(), Arg.Any<int>());
    }

    [Fact]
    public void IsInconsistentFullTimeScoreWhenMuayThaiAndValidScoresShouldCallHelper()
    {
        // Arrange
        var match = CreateTestMatch();
        match.SportType = SportType.MuayThai;
        match.FinalHomeScore = 1;
        match.FinalAwayScore = 0;
        var resultDetails = new List<ResultDetail>
        {
            new() { MatchId = 1, SourceId = SourceType.Sbo, ResourceId = ResourceType.Final, HomeValue = "5", AwayValue = "0" }
        };

        muayThaiHelperService.MapSboToSettle(5, 0).Returns(("1", "0", ""));

        // Act
        var result = service.IsInconsistentFullTimeScore(match, resultDetails);

        // Assert
        Assert.False(result);
        muayThaiHelperService.Received(1).MapSboToSettle(5, 0);
    }

    private static Match CreateTestMatch()
    {
        return new Match
        {
            MatchId = 1,
            SportType = SportType.Soccer,
            FinalHomeScore = 2,
            FinalAwayScore = 1,
            HalfTimeHomeScore = 1,
            HalfTimeAwayScore = 0,
            SuspenderFullTime = "-",
            SuspenderHalfTime = "-"
        };
    }
}