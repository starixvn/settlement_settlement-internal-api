using Settlement.Gaming.Criteria.Happy5;
using Settlement.Gaming.Models.Happy5;

namespace Settlement.Gaming.Services.Happy5;

public interface LeagueService
{
    Task<List<LeaguesModel>> Search(GetMatchesCriteria criteria);
}

public class LeagueServiceImp(
    MatchService matchService,
    IRefDataService refDataService)
    : LeagueService
{
    public async Task<List<LeaguesModel>> Search(GetMatchesCriteria criteria)
    {
        var matches = await matchService.Matches(criteria);

        return await CreateLeagues(matches);
    }

    private async Task<List<LeaguesModel>> CreateLeagues(IReadOnlyCollection<MatchDetail> matches)
    {
        var leagueIds = matches.GroupBy(m => m.LeagueId).Select(m => m.Key).ToList();
        var leagues = new List<LeaguesModel>();
        foreach (var leagueId in leagueIds)
        {
            var matchesByLeagueId = matches.Where(m => m.LeagueId == leagueId).ToList();
            var league = await LeagueItem(leagueId, matchesByLeagueId);
            leagues.Add(league);
        }

        return leagues;
    }

    private async Task<LeaguesModel> LeagueItem(int leagueId, List<MatchDetail> matches)
    {
        var league = new LeaguesModel
        {
            LeagueId = leagueId,
            Matches = matches,
            RefDataService = refDataService
        };

        return await Task.Run(() => league);
    }
}