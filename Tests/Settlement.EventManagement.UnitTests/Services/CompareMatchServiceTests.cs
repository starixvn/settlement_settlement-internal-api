using Settlement.EventManagement.Models;
using Settlement.EventManagement.Services;

namespace Settlement.EventManagement.UnitTests.Services;

public class CompareMatchServiceTests
{
    private readonly CompareMatchService service;

    public CompareMatchServiceTests()
    {
        service = new CompareMatchServiceImp();
    }

    [Fact]
    public void GetDifferentMatchSiteInfoWhenSiteMatchNotFoundShouldReturnNull()
    {
        // Arrange
        var match = CreateTestMatch(1);
        var siteMatches = new List<Match>();

        // Act
        var result = service.GetDifferentMatchSiteInfo(match, siteMatches);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetDifferentMatchSiteInfoWhenNoChangesDetectedShouldReturnNull()
    {
        // Arrange
        var match = CreateTestMatch(1);
        var siteMatch = CreateTestMatch(1);
        var siteMatches = new List<Match> { siteMatch };

        // Act
        var result = service.GetDifferentMatchSiteInfo(match, siteMatches);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetDifferentMatchSiteInfoWhenCalculatingSingleChangedShouldReturnSiteMatch()
    {
        // Arrange
        var match = CreateTestMatch(1);
        var siteMatch = CreateTestMatch(1);
        siteMatch.IsCalculatingSingle = match.IsCalculatingSingle == 0 ? 1 : 0;
        var siteMatches = new List<Match> { siteMatch };

        // Act
        var result = service.GetDifferentMatchSiteInfo(match, siteMatches);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(siteMatch.IsCalculatingSingle, result.IsCalculatingSingle);
        Assert.Equal(siteMatch.IsCalculatingMP, result.IsCalculatingMP);
        Assert.Equal(siteMatch.IsExistMarkedTicket, result.IsExistMarkedTicket);
        Assert.Equal(siteMatch.IsExistRunningTicket, result.IsExistRunningTicket);
        Assert.Equal(siteMatch.IsExistEarlyBetTypes, result.IsExistEarlyBetTypes);
        Assert.Equal(siteMatch.IsRunningEarlyBetTypes, result.IsRunningEarlyBetTypes);
        Assert.Equal(siteMatch.IsWaitingEarlyBetTypes, result.IsWaitingEarlyBetTypes);
    }

    [Fact]
    public void GetDifferentMatchSiteInfoWhenCalculatingMPChangedShouldReturnSiteMatch()
    {
        // Arrange
        var match = CreateTestMatch(1);
        var siteMatch = CreateTestMatch(1);
        siteMatch.IsCalculatingMP = match.IsCalculatingMP == 0 ? 1 : 0;
        var siteMatches = new List<Match> { siteMatch };

        // Act
        var result = service.GetDifferentMatchSiteInfo(match, siteMatches);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(siteMatch.IsCalculatingMP, result.IsCalculatingMP);
    }

    [Fact]
    public void GetDifferentMatchSiteInfoWhenExistMarkedTicketChangedShouldReturnSiteMatch()
    {
        // Arrange
        var match = CreateTestMatch(1);
        var siteMatch = CreateTestMatch(1);
        siteMatch.IsExistMarkedTicket = !match.IsExistMarkedTicket;
        var siteMatches = new List<Match> { siteMatch };

        // Act
        var result = service.GetDifferentMatchSiteInfo(match, siteMatches);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(siteMatch.IsExistMarkedTicket, result.IsExistMarkedTicket);
    }

    [Fact]
    public void GetDifferentMatchSiteInfoWhenExistRunningTicketChangedShouldReturnSiteMatch()
    {
        // Arrange
        var match = CreateTestMatch(1);
        var siteMatch = CreateTestMatch(1);
        siteMatch.IsExistRunningTicket = !match.IsExistRunningTicket;
        var siteMatches = new List<Match> { siteMatch };

        // Act
        var result = service.GetDifferentMatchSiteInfo(match, siteMatches);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(siteMatch.IsExistRunningTicket, result.IsExistRunningTicket);
    }

    [Fact]
    public void GetDifferentMatchSiteInfoWhenExistEarlyBetTypesChangedShouldReturnSiteMatch()
    {
        // Arrange
        var match = CreateTestMatch(1);
        var siteMatch = CreateTestMatch(1);
        siteMatch.IsExistEarlyBetTypes = !match.IsExistEarlyBetTypes;
        var siteMatches = new List<Match> { siteMatch };

        // Act
        var result = service.GetDifferentMatchSiteInfo(match, siteMatches);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(siteMatch.IsExistEarlyBetTypes, result.IsExistEarlyBetTypes);
    }

    [Fact]
    public void GetDifferentMatchSiteInfoWhenRunningEarlyBetTypesChangedShouldReturnSiteMatch()
    {
        // Arrange
        var match = CreateTestMatch(1);
        var siteMatch = CreateTestMatch(1);
        siteMatch.IsRunningEarlyBetTypes = !match.IsRunningEarlyBetTypes;
        var siteMatches = new List<Match> { siteMatch };

        // Act
        var result = service.GetDifferentMatchSiteInfo(match, siteMatches);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(siteMatch.IsRunningEarlyBetTypes, result.IsRunningEarlyBetTypes);
    }

    [Fact]
    public void GetDifferentMatchSiteInfoWhenWaitingEarlyBetTypesChangedShouldReturnSiteMatch()
    {
        // Arrange
        var match = CreateTestMatch(1);
        var siteMatch = CreateTestMatch(1);
        siteMatch.IsWaitingEarlyBetTypes = !match.IsWaitingEarlyBetTypes;
        var siteMatches = new List<Match> { siteMatch };

        // Act
        var result = service.GetDifferentMatchSiteInfo(match, siteMatches);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(siteMatch.IsWaitingEarlyBetTypes, result.IsWaitingEarlyBetTypes);
    }

    [Fact]
    public void GetDifferentMatchSiteInfoWhenMultiplePropertiesChangedShouldReturnSiteMatch()
    {
        // Arrange
        var match = CreateTestMatch(1);
        var siteMatch = CreateTestMatch(1);
        siteMatch.IsCalculatingSingle = match.IsCalculatingSingle == 0 ? 1 : 0;
        siteMatch.IsExistMarkedTicket = !match.IsExistMarkedTicket;
        siteMatch.IsRunningEarlyBetTypes = !match.IsRunningEarlyBetTypes;
        var siteMatches = new List<Match> { siteMatch };

        // Act
        var result = service.GetDifferentMatchSiteInfo(match, siteMatches);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(siteMatch.IsCalculatingSingle, result.IsCalculatingSingle);
        Assert.Equal(siteMatch.IsExistMarkedTicket, result.IsExistMarkedTicket);
        Assert.Equal(siteMatch.IsRunningEarlyBetTypes, result.IsRunningEarlyBetTypes);
    }

    [Fact]
    public void GetDifferentMatchSiteInfoWhenMultipleSiteMatchesExistShouldFindCorrectMatch()
    {
        // Arrange
        var match = CreateTestMatch(1);
        var siteMatch1 = CreateTestMatch(1);
        var siteMatch2 = CreateTestMatch(2);
        siteMatch1.IsCalculatingSingle = match.IsCalculatingSingle == 0 ? 1 : 0;
        var siteMatches = new List<Match> { siteMatch2, siteMatch1 };

        // Act
        var result = service.GetDifferentMatchSiteInfo(match, siteMatches);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(siteMatch1.IsCalculatingSingle, result.IsCalculatingSingle);
    }

    [Fact]
    public void GetDifferentMatchSiteInfoWhenAllTicketPropertiesChangedShouldReturnSiteMatch()
    {
        // Arrange
        var match = CreateTestMatch(1);
        var siteMatch = CreateTestMatch(1);
        siteMatch.IsCalculatingSingle = match.IsCalculatingSingle == 0 ? 1 : 0;
        siteMatch.IsCalculatingMP = match.IsCalculatingMP == 0 ? 1 : 0;
        siteMatch.IsExistMarkedTicket = !match.IsExistMarkedTicket;
        siteMatch.IsExistRunningTicket = !match.IsExistRunningTicket;
        var siteMatches = new List<Match> { siteMatch };

        // Act
        var result = service.GetDifferentMatchSiteInfo(match, siteMatches);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(siteMatch.IsCalculatingSingle, result.IsCalculatingSingle);
        Assert.Equal(siteMatch.IsCalculatingMP, result.IsCalculatingMP);
        Assert.Equal(siteMatch.IsExistMarkedTicket, result.IsExistMarkedTicket);
        Assert.Equal(siteMatch.IsExistRunningTicket, result.IsExistRunningTicket);
    }

    [Fact]
    public void GetDifferentMatchSiteInfoWhenAllSpecialBetTypePropertiesChangedShouldReturnSiteMatch()
    {
        // Arrange
        var match = CreateTestMatch(1);
        var siteMatch = CreateTestMatch(1);
        siteMatch.IsExistEarlyBetTypes = !match.IsExistEarlyBetTypes;
        siteMatch.IsRunningEarlyBetTypes = !match.IsRunningEarlyBetTypes;
        siteMatch.IsWaitingEarlyBetTypes = !match.IsWaitingEarlyBetTypes;
        var siteMatches = new List<Match> { siteMatch };

        // Act
        var result = service.GetDifferentMatchSiteInfo(match, siteMatches);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(siteMatch.IsExistEarlyBetTypes, result.IsExistEarlyBetTypes);
        Assert.Equal(siteMatch.IsRunningEarlyBetTypes, result.IsRunningEarlyBetTypes);
        Assert.Equal(siteMatch.IsWaitingEarlyBetTypes, result.IsWaitingEarlyBetTypes);
    }

    [Fact]
    public void GetDifferentMatchSiteInfoWhenMatchIdDoesNotMatchShouldReturnNull()
    {
        // Arrange
        var match = CreateTestMatch(1);
        var siteMatch = CreateTestMatch(2);
        siteMatch.IsCalculatingSingle = match.IsCalculatingSingle == 0 ? 1 : 0;
        var siteMatches = new List<Match> { siteMatch };

        // Act
        var result = service.GetDifferentMatchSiteInfo(match, siteMatches);

        // Assert
        Assert.Null(result);
    }

    private static Match CreateTestMatch(int matchId)
    {
        return new Match
        {
            MatchId = matchId,
            IsCalculatingSingle = 0,
            IsCalculatingMP = 0,
            IsExistMarkedTicket = false,
            IsExistRunningTicket = false,
            IsExistEarlyBetTypes = false,
            IsRunningEarlyBetTypes = false,
            IsWaitingEarlyBetTypes = false
        };
    }
}
