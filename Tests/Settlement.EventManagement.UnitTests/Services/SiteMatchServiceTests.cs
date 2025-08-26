using Settlement.EventManagement.Constants;
using Settlement.EventManagement.Models;
using Settlement.EventManagement.Services;

namespace Settlement.EventManagement.UnitTests.Services;

public class SiteMatchServiceTests
{
    private readonly SiteMatchService service;

    public SiteMatchServiceTests()
    {
        service = new SiteMatchServiceImp();
    }

    [Fact]
    public void IsExistMarkedTicketsWhenMatchHasMarkedTicketShouldReturnTrue()
    {
        // Arrange
        var match = CreateTestMatch();
        match.IsExistMarkedTicket = true;

        // Act
        var result = service.IsExistMarkedTickets(match);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsExistMarkedTicketsWhenMatchHasNoMarkedTicketButSiteMatchHasShouldReturnTrue()
    {
        // Arrange
        var match = CreateTestMatch();
        match.IsExistMarkedTicket = false;
        match.SiteMatch.Add(new SiteMatch { IsExistMarkedTicket = true });

        // Act
        var result = service.IsExistMarkedTickets(match);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsExistMarkedTicketsWhenMatchAndSiteMatchHaveNoMarkedTicketsShouldReturnFalse()
    {
        // Arrange
        var match = CreateTestMatch();
        match.IsExistMarkedTicket = false;
        match.SiteMatch.Add(new SiteMatch { IsExistMarkedTicket = false });

        // Act
        var result = service.IsExistMarkedTickets(match);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsExistMarkedTicketsWhenMatchHasMarkedTicketAndSiteMatchAlsoHasShouldReturnTrue()
    {
        // Arrange
        var match = CreateTestMatch();
        match.IsExistMarkedTicket = true;
        match.SiteMatch.Add(new SiteMatch { IsExistMarkedTicket = true });

        // Act
        var result = service.IsExistMarkedTickets(match);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void GetBackgroundCompletedStatusWhenOnlyMarkedTicketShouldReturnWhiteBlinking()
    {
        // Arrange
        var match = CreateTestMatch();
        match.IsExistMarkedTicket = true;
        match.IsExistRunningTicket = false;
        match.SiteMatch.Add(new SiteMatch { IsExistMarkedTicket = false, IsExistRunningTicket = false });

        // Act
        var result = service.GetBackgroundCompletedStatus(match);

        // Assert
        Assert.Equal(Background.WhiteBlinking, result);
    }

    [Fact]
    public void GetBackgroundCompletedStatusWhenMarkedAndRunningTicketsShouldReturnPinkBlinking()
    {
        // Arrange
        var match = CreateTestMatch();
        match.IsExistMarkedTicket = true;
        match.IsExistRunningTicket = true;

        // Act
        var result = service.GetBackgroundCompletedStatus(match);

        // Assert
        Assert.Equal(Background.PinkBlinking, result);
    }

    [Fact]
    public void GetBackgroundCompletedStatusWhenOnlyRunningTicketsShouldReturnPink()
    {
        // Arrange
        var match = CreateTestMatch();
        match.IsExistMarkedTicket = false;
        match.IsExistRunningTicket = true;

        // Act
        var result = service.GetBackgroundCompletedStatus(match);

        // Assert
        Assert.Equal(Background.Pink, result);
    }

    [Fact]
    public void GetBackgroundCompletedStatusWhenNoTicketsShouldReturnEmptyString()
    {
        // Arrange
        var match = CreateTestMatch();
        match.IsExistMarkedTicket = false;
        match.IsExistRunningTicket = false;

        // Act
        var result = service.GetBackgroundCompletedStatus(match);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void GetBackgroundCompletedStatusWhenSiteMatchHasRunningTicketsShouldReturnPink()
    {
        // Arrange
        var match = CreateTestMatch();
        match.IsExistMarkedTicket = false;
        match.IsExistRunningTicket = false;
        match.SiteMatch.Add(new SiteMatch { IsExistRunningTicket = true });

        // Act
        var result = service.GetBackgroundCompletedStatus(match);

        // Assert
        Assert.Equal(Background.Pink, result);
    }

    [Fact]
    public void EarlySettlementStatusWhenNoEarlyBetTypesAndChildMatchTypeIsMatchPenShouldReturnNoTicket()
    {
        // Arrange
        var match = CreateTestMatch();
        match.ChildMatchType = 202; // MatchPen constant
        match.IsExistEarlyBetTypes = false;
        match.IsOpenOdds = false;

        // Act
        var result = service.EarlySettlementStatus(match);

        // Assert
        Assert.Equal(TicketStatus.NoTicket, result);
    }

    [Fact]
    public void EarlySettlementStatusWhenNoEarlyBetTypesAndIsOpenOddsShouldReturnNoTicket()
    {
        // Arrange
        var match = CreateTestMatch();
        match.ChildMatchType = 100; // Not MatchPen
        match.IsExistEarlyBetTypes = false;
        match.IsOpenOdds = true;

        // Act
        var result = service.EarlySettlementStatus(match);

        // Assert
        Assert.Equal(TicketStatus.NoTicket, result);
    }

    [Fact]
    public void EarlySettlementStatusWhenNoEarlyBetTypesAndNotMatchPenOrOpenOddsShouldReturnNormal()
    {
        // Arrange
        var match = CreateTestMatch();
        match.ChildMatchType = 100; // Not MatchPen
        match.IsExistEarlyBetTypes = false;
        match.IsOpenOdds = false;

        // Act
        var result = service.EarlySettlementStatus(match);

        // Assert
        Assert.Equal(TicketStatus.Normal, result);
    }

    [Fact]
    public void EarlySettlementStatusWhenRunningEarlyBetTypesShouldReturnRunning()
    {
        // Arrange
        var match = CreateTestMatch();
        match.IsExistEarlyBetTypes = true;
        match.IsRunningEarlyBetTypes = true;
        match.IsWaitingEarlyBetTypes = false;

        // Act
        var result = service.EarlySettlementStatus(match);

        // Assert
        Assert.Equal(TicketStatus.Running, result);
    }

    [Fact]
    public void EarlySettlementStatusWhenWaitingEarlyBetTypesShouldReturnWaiting()
    {
        // Arrange
        var match = CreateTestMatch();
        match.IsExistEarlyBetTypes = true;
        match.IsRunningEarlyBetTypes = false;
        match.IsWaitingEarlyBetTypes = true;

        // Act
        var result = service.EarlySettlementStatus(match);

        // Assert
        Assert.Equal(TicketStatus.Waiting, result);
    }

    [Fact]
    public void EarlySettlementStatusWhenEarlyBetTypesExistButNotRunningOrWaitingShouldReturnCompleted()
    {
        // Arrange
        var match = CreateTestMatch();
        match.IsExistEarlyBetTypes = true;
        match.IsRunningEarlyBetTypes = false;
        match.IsWaitingEarlyBetTypes = false;

        // Act
        var result = service.EarlySettlementStatus(match);

        // Assert
        Assert.Equal(TicketStatus.Completed, result);
    }

    [Fact]
    public void EarlySettlementStatusWhenSiteMatchHasEarlyBetTypesShouldReturnRunning()
    {
        // Arrange
        var match = CreateTestMatch();
        match.IsExistEarlyBetTypes = false;
        match.IsRunningEarlyBetTypes = false;
        match.SiteMatch.Add(new SiteMatch { IsExistEarlyBetTypes = true, IsRunningEarlyBetTypes = true });

        // Act
        var result = service.EarlySettlementStatus(match);

        // Assert
        Assert.Equal(TicketStatus.Running, result);
    }

    [Fact]
    public void EarlySettlementStatusWhenSiteMatchHasWaitingEarlyBetTypesShouldReturnWaiting()
    {
        // Arrange
        var match = CreateTestMatch();
        match.IsExistEarlyBetTypes = false;
        match.IsWaitingEarlyBetTypes = false;
        match.SiteMatch.Add(new SiteMatch { IsExistEarlyBetTypes = true, IsWaitingEarlyBetTypes = true });

        // Act
        var result = service.EarlySettlementStatus(match);

        // Assert
        Assert.Equal(TicketStatus.Waiting, result);
    }

    [Fact]
    public void EarlySettlementStatusWhenSiteMatchHasEarlyBetTypesButNotRunningOrWaitingShouldReturnCompleted()
    {
        // Arrange
        var match = CreateTestMatch();
        match.IsExistEarlyBetTypes = false;
        match.SiteMatch.Add(new SiteMatch { IsExistEarlyBetTypes = true, IsRunningEarlyBetTypes = false, IsWaitingEarlyBetTypes = false });

        // Act
        var result = service.EarlySettlementStatus(match);

        // Assert
        Assert.Equal(TicketStatus.Completed, result);
    }

    [Fact]
    public void EarlySettlementStatusWhenBothMatchAndSiteMatchHaveEarlyBetTypesShouldReturnRunning()
    {
        // Arrange
        var match = CreateTestMatch();
        match.IsExistEarlyBetTypes = true;
        match.IsRunningEarlyBetTypes = true;
        match.SiteMatch.Add(new SiteMatch { IsExistEarlyBetTypes = true, IsRunningEarlyBetTypes = false });

        // Act
        var result = service.EarlySettlementStatus(match);

        // Assert
        Assert.Equal(TicketStatus.Running, result);
    }

    [Fact]
    public void EarlySettlementStatusWhenSiteMatchHasRunningTicketsShouldReturnPink()
    {
        // Arrange
        var match = CreateTestMatch();
        match.IsExistMarkedTicket = false;
        match.IsExistRunningTicket = false;
        match.SiteMatch.Add(new SiteMatch { IsExistRunningTicket = true });

        // Act
        var result = service.GetBackgroundCompletedStatus(match);

        // Assert
        Assert.Equal(Background.Pink, result);
    }

    [Fact]
    public void EarlySettlementStatusWhenSiteMatchHasMarkedTicketsShouldReturnWhiteBlinking()
    {
        // Arrange
        var match = CreateTestMatch();
        match.IsExistMarkedTicket = true;
        match.IsExistRunningTicket = false;
        match.SiteMatch.Add(new SiteMatch { IsExistMarkedTicket = false, IsExistRunningTicket = false });

        // Act
        var result = service.GetBackgroundCompletedStatus(match);

        // Assert
        Assert.Equal(Background.WhiteBlinking, result);
    }

    [Fact]
    public void EarlySettlementStatusWhenSiteMatchHasBothMarkedAndRunningTicketsShouldReturnPinkBlinking()
    {
        // Arrange
        var match = CreateTestMatch();
        match.IsExistMarkedTicket = true;
        match.IsExistRunningTicket = true;

        // Act
        var result = service.GetBackgroundCompletedStatus(match);

        // Assert
        Assert.Equal(Background.PinkBlinking, result);
    }

    private static Match CreateTestMatch()
    {
        return new Match
        {
            MatchId = 1,
            IsExistMarkedTicket = false,
            IsExistRunningTicket = false,
            IsExistEarlyBetTypes = false,
            IsRunningEarlyBetTypes = false,
            IsWaitingEarlyBetTypes = false,
            IsOpenOdds = false,
            ChildMatchType = 0,
            SiteMatch = new List<SiteMatch>()
        };
    }
}
