using Fanex.Data.Repository;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using NSubstitute;
using Settlement.EventManagement.Criteria;
using Settlement.EventManagement.Models;
using Settlement.EventManagement.Services;
using Settlement.Internal.Models;
using Settlement.Internal.Services;

namespace Settlement.EventManagement.UnitTests.Services;

public class MultiSportServiceTests
{
    private readonly MultiSportService service;

    private readonly IDynamicRepository repository;

    private readonly MatchService matchService;

    private readonly IRefDataService refDataService;

    public MultiSportServiceTests()
    {
        repository = Substitute.For<IDynamicRepository>();
        matchService = Substitute.For<MatchService>();

        var cacheService = Substitute.For<IMemoryCache>();
        var appSetting = new AppSettingsKey { SiteDbServer = ["DB01", "DB02"] };
        var option = Options.Create(appSetting);
        refDataService = Substitute.For<IRefDataService>();
        service = new MultiSportServiceImp(repository, matchService, cacheService, option);
    }

    [Fact]
    public async Task GetWaitingMatches_NotExistMatchesOnCache_ReturnQueriedMatches()
    {
        // Arrange
        var match = new Match
        {
            MatchId = 700000,
            SportType = 2,
        };
        var resultDetail = new ResultDetail
        {
            MatchId = 700000,
            ResourceId = "01"
        };

        var listMatch = new List<Match> { match };

        var criteria = new GetWaitingMultiSportCriteria
        {
            LeagueIds = "1,2"
        };

        repository.FetchAsync<Match>(Arg.Any<GetWaitingMultiSportCriteria>()).Returns(listMatch);
        repository.FetchAsync<ResultDetail>(Arg.Any<ICriteria>()).Returns(new List<ResultDetail> { resultDetail });
        refDataService.GetLeagueName(Arg.Any<string>()).Returns(string.Empty);
        matchService.GetScoreForMatchBySportType(Arg.Any<List<Match>>(), Arg.Any<List<ResultDetail>>(), Arg.Any<int>()).Returns(
            [match]);
        matchService.SetNewMatchInfo(Arg.Any<List<Match>>(), Arg.Any<List<ResultDetail>>()).Returns(listMatch);

        // Act
        var waitingMatches = await service.GetWaitingMatches(criteria);

        // Assert
        Assert.NotNull(waitingMatches);
    }
}