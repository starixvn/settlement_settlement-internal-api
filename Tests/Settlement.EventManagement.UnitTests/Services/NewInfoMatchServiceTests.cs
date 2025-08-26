using Fanex.Data.Repository;
using NSubstitute;
using Settlement.EventManagement.Constants;
using Settlement.EventManagement.Criteria;
using Settlement.EventManagement.Models;
using Settlement.EventManagement.Services;
using Settlement.Internal.Constants;

namespace Settlement.EventManagement.UnitTests.Services;

public class NewInfoMatchServiceTests
{
    private readonly NewInfoMatchService service;
    private readonly SiteMatchService siteMatchService;
    private readonly AdjustEventStatusService adjustEventStatusService;
    private readonly IDynamicRepository repository;

    public NewInfoMatchServiceTests()
    {
        siteMatchService = Substitute.For<SiteMatchService>();
        adjustEventStatusService = Substitute.For<AdjustEventStatusService>();
        repository = Substitute.For<IDynamicRepository>();
        service = new NewInfoMatchServiceImp(siteMatchService, adjustEventStatusService, repository);
    }

    [Fact]
    public void EventStatusColorWhenEventStatusIsPostponedAndHasMarkedTicketsShouldReturnYellowBlinking()
    {
        // Arrange
        var match = CreateTestMatch();
        match.EventStatus = EventStatus.Postponed;
        siteMatchService.IsExistMarkedTickets(match).Returns(true);

        // Act
        var result = service.EventStatusColor(match);

        // Assert
        Assert.Equal(Background.YellowBlinking, result);
    }

    [Fact]
    public void EventStatusColorWhenEventStatusIsPostponedAndNoMarkedTicketsShouldReturnYellow()
    {
        // Arrange
        var match = CreateTestMatch();
        match.EventStatus = EventStatus.Postponed;
        siteMatchService.IsExistMarkedTickets(match).Returns(false);

        // Act
        var result = service.EventStatusColor(match);

        // Assert
        Assert.Equal(Background.Yellow, result);
    }

    [Fact]
    public void EventStatusColorWhenEventStatusIsCompletedShouldReturnBackgroundCompletedStatus()
    {
        // Arrange
        var match = CreateTestMatch();
        match.EventStatus = EventStatus.Completed;
        var expectedBackground = Background.PinkBlinking;
        siteMatchService.GetBackgroundCompletedStatus(match).Returns(expectedBackground);

        // Act
        var result = service.EventStatusColor(match);

        // Assert
        Assert.Equal(expectedBackground, result);
        siteMatchService.Received(1).GetBackgroundCompletedStatus(match);
    }

    [Fact]
    public void EventStatusColorWhenRemarkIsPendingAndHasMarkedTicketsShouldReturnBrightYellowBlinking()
    {
        // Arrange
        var match = CreateTestMatch();
        match.SettlementStatus = 1; // Odd number makes Remark.IsPending true
        match.EventStatus = "some other status"; // Not Postponed or Completed
        siteMatchService.IsExistMarkedTickets(match).Returns(true);

        // Act
        var result = service.EventStatusColor(match);

        // Assert
        Assert.Equal(Background.BrightYellowBlinking, result);
    }

    [Fact]
    public void EventStatusColorWhenRemarkIsPendingAndNoMarkedTicketsShouldReturnBrightYellow()
    {
        // Arrange
        var match = CreateTestMatch();
        match.SettlementStatus = 1; // Odd number makes Remark.IsPending true
        match.EventStatus = "some other status"; // Not Postponed or Completed
        siteMatchService.IsExistMarkedTickets(match).Returns(false);

        // Act
        var result = service.EventStatusColor(match);

        // Assert
        Assert.Equal(Background.BrightYellow, result);
    }

    [Fact]
    public void EventStatusColorWhenDefaultCaseAndHasMarkedTicketsShouldReturnWhiteBlinking()
    {
        // Arrange
        var match = CreateTestMatch();
        match.EventStatus = "some other status";
        match.SettlementStatus = SettlementStatus.Completed; // This makes Remark.IsPending false
        siteMatchService.IsExistMarkedTickets(match).Returns(true);

        // Act
        var result = service.EventStatusColor(match);

        // Assert
        Assert.Equal(Background.WhiteBlinking, result);
    }

    [Fact]
    public void EventStatusColorWhenDefaultCaseAndNoMarkedTicketsShouldReturnEmptyString()
    {
        // Arrange
        var match = CreateTestMatch();
        match.EventStatus = "some other status";
        match.SettlementStatus = SettlementStatus.Completed; // This makes Remark.IsPending false
        siteMatchService.IsExistMarkedTickets(match).Returns(false);

        // Act
        var result = service.EventStatusColor(match);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void NewEventStatusWhenSportTypeSupportsSettleDetermineShouldCallAdjustEventStatusService()
    {
        // Arrange
        var match = CreateTestMatch();
        match.SportType = SportType.BasketBall; // Supports settle determine
        var resultDetails = new List<ResultDetail>
        {
            new()
        };
        const string expectedStatus = "adjusted status";
        adjustEventStatusService.EventStatusAbandoned(match.EventStatus, resultDetails, match.SportType).Returns(expectedStatus);

        // Act
        var result = service.NewEventStatus(match, resultDetails);

        // Assert
        Assert.Equal(expectedStatus, result);
        adjustEventStatusService.Received(1).EventStatusAbandoned(match.EventStatus, resultDetails, match.SportType);
    }

    [Fact]
    public void NewEventStatusWhenSportTypeDoesNotSupportSettleDetermineShouldReturnOriginalEventStatus()
    {
        // Arrange
        var match = CreateTestMatch();
        match.SportType = SportType.Soccer; // Does not support settle determine
        var resultDetails = new List<ResultDetail>
        {
            new()
        };

        // Act
        var result = service.NewEventStatus(match, resultDetails);

        // Assert
        Assert.Equal(match.EventStatus, result);
        adjustEventStatusService.DidNotReceive().EventStatusAbandoned(Arg.Any<string>(), Arg.Any<IEnumerable<ResultDetail>>(), Arg.Any<int>());
    }

    [Fact]
    public void NewEventStatusWhenIsCalculatingSingleIsWaitingShouldReturnWaiting()
    {
        // Arrange
        var match = CreateTestMatch();
        match.IsCalculatingSingle = CalculateStatus.IsWaiting;

        // Act
        var result = service.NewEventStatus(match);

        // Assert
        Assert.Equal(EventStatus.Waiting, result);
    }

    [Fact]
    public void NewEventStatusWhenIsCalculatingSingleIsCalculatingShouldReturnCalculating()
    {
        // Arrange
        var match = CreateTestMatch();
        match.IsCalculatingSingle = CalculateStatus.IsCalculating;

        // Act
        var result = service.NewEventStatus(match);

        // Assert
        Assert.Equal(EventStatus.Calculating, result);
    }

    [Fact]
    public void NewEventStatusWhenIsCalculatingSingleIsNotWaitingOrCalculatingShouldReturnOriginalEventStatus()
    {
        // Arrange
        var match = CreateTestMatch();
        match.IsCalculatingSingle = 2; // Some other value

        // Act
        var result = service.NewEventStatus(match);

        // Assert
        Assert.Equal(match.EventStatus, result);
    }

    [Fact]
    public void EarlySettlementStatusShouldCallSiteMatchService()
    {
        // Arrange
        var match = CreateTestMatch();
        var expectedStatus = TicketStatus.Running;
        siteMatchService.EarlySettlementStatus(match).Returns(expectedStatus);

        // Act
        var result = service.EarlySettlementStatus(match);

        // Assert
        Assert.Equal(expectedStatus, result);
        siteMatchService.Received(1).EarlySettlementStatus(match);
    }

    [Fact]
    public void ConfirmScoreWhenResultDetailFoundShouldReturnFormattedScore()
    {
        // Arrange
        var resultDetails = new List<ResultDetail>
        {
            new()
            {
                ResourceId = "01",
                HomeValue = "2",
                AwayValue = "1",
                SourceName = "SBO"
            }
        };
        var resource = "01";

        // Act
        var result = service.ConfirmScore(resultDetails, resource);

        // Assert
        Assert.Equal("(SBO: 2-1)", result);
    }

    [Fact]
    public void ConfirmScoreWhenResultDetailNotFoundShouldReturnEmptyString()
    {
        // Arrange
        var resultDetails = new List<ResultDetail>
        {
            new()
            {
                ResourceId = "02",
                HomeValue = "2",
                AwayValue = "1",
                SourceName = "SBO"
            }
        };
        var resource = "01";

        // Act
        var result = service.ConfirmScore(resultDetails, resource);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void ConfirmScoreWhenResultDetailHasEmptyValuesShouldReturnEmptyString()
    {
        // Arrange
        var resultDetails = new List<ResultDetail>
        {
            new()
            {
                ResourceId = "01",
                HomeValue = "",
                AwayValue = "1",
                SourceName = "SBO"
            }
        };
        var resource = "01";

        // Act
        var result = service.ConfirmScore(resultDetails, resource);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public async Task GetAutoSettingShouldReturnMatchesWithAutoSettings()
    {
        // Arrange
        var matches = new List<Match>
        {
            CreateTestMatch(),
            CreateTestMatch(2)
        };

        var matchStatuses = new List<MatchStatusModel>
        {
            new() { MatchId = 1, AutoPayoutStatus = 1, InstantStatus = 2 },
            new() { MatchId = 2, AutoPayoutStatus = 3, InstantStatus = 4 }
        };

        var failedAutoResources = new List<FailedAutoResource>
        {
            new() { MatchId = 1, ResourceId = "01" },
            new() { MatchId = 2, ResourceId = "02" }
        };

        repository.FetchMultipleAsync<MatchStatusModel, FailedAutoResource>(Arg.Any<GetAutoStatusCriteria>())
            .Returns((matchStatuses, failedAutoResources));

        // Act
        var result = await service.GetAutoSetting(matches);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal(1, result[0].AutoPayoutStatus);
        Assert.Equal(2, result[0].AutoInstantStatus);
        Assert.Equal(3, result[1].AutoPayoutStatus);
        Assert.Equal(4, result[1].AutoInstantStatus);
        Assert.Single(result[0].FailedAutoResources);
        Assert.Single(result[1].FailedAutoResources);
    }

    [Fact]
    public async Task GetAutoSettingWhenNoMatchesShouldReturnEmptyList()
    {
        // Arrange
        var matches = new List<Match>();

        repository.FetchMultipleAsync<MatchStatusModel, FailedAutoResource>(Arg.Any<GetAutoStatusCriteria>())
            .Returns((new List<MatchStatusModel>(), new List<FailedAutoResource>()));

        // Act
        var result = await service.GetAutoSetting(matches);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAutoSettingWhenNoMatchStatusesShouldReturnEmptyList()
    {
        // Arrange
        var matches = new List<Match>
        {
            CreateTestMatch(),
            CreateTestMatch(2)
        };

        repository.FetchMultipleAsync<MatchStatusModel, FailedAutoResource>(Arg.Any<GetAutoStatusCriteria>())
            .Returns((new List<MatchStatusModel>(), new List<FailedAutoResource>()));

        // Act
        var result = await service.GetAutoSetting(matches);

        // Assert
        Assert.Empty(result);
    }

    private static Match CreateTestMatch(int matchId = 1)
    {
        return new Match
        {
            MatchId = matchId,
            EventStatus = EventStatus.Completed,
            SettlementStatus = SettlementStatus.Completed,
            IsCalculatingSingle = 2, // Not IsWaiting (0) or IsCalculating (1)
            SiteMatch = new List<SiteMatch>()
        };
    }
}