using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Settlement.EventManagement.Criteria;
using Settlement.Internal.Constants;
using Settlement.Internal.Models;
using System.Security.Cryptography;

namespace Settlement.EventManagement.Services;

using Models;
using System.Text;

public interface MultiSportService
{
    Task<List<Match>> GetWaitingMatches(GetWaitingMultiSportCriteria criteria);
}

public class MultiSportServiceImp(
    IDynamicRepository repository,
    MatchService matchService,
    IMapper mapper,
    IMemoryCache cacheService,
    IOptions<AppSettingsKey> appSettingsKey)
    : MultiSportService
{
    private static int _isProcessing;

    public async Task<List<Match>> GetWaitingMatches(GetWaitingMultiSportCriteria criteria)
    {
        var hashLeagueIds = ConvertKeyToHash(criteria.LeagueIds);
        var matches = MatchesOnShortCaching(hashLeagueIds);

        if (matches is { Count: > 0 })
        {
            return matches;
        }

        if (0 == Interlocked.Exchange(ref _isProcessing, 1))
        {
            try
            {
                return await GetAndCachingMatches(criteria, hashLeagueIds);
            }
            finally
            {
                Interlocked.Exchange(ref _isProcessing, 0);
            }
        }

        return MatchesOnLongCaching(hashLeagueIds);
    }

    private static string ConvertKeyToHash(string input)
    {
        var data = SHA1.HashData(Encoding.Unicode.GetBytes(input));
        return Convert.ToHexString(data).ToLowerInvariant();
    }

    private List<Match> MatchesOnShortCaching(string hashLeagueIds)
    {
        var shortCachingKey = ShortCachingKey(hashLeagueIds);
        return cacheService.Get<List<Match>>(shortCachingKey);
    }

    private List<Match> MatchesOnLongCaching(string hashLeagueIds)
    {
        var longCachingKey = LongCachingKey(hashLeagueIds);
        return cacheService.Get<List<Match>>(longCachingKey);
    }

    private static string ShortCachingKey(string hashLeagueIds) => $"{CacheSettings.Keys.MemoryCacheMultiSportCacheKey}-{hashLeagueIds}";

    private static string LongCachingKey(string hashLeagueIds) => $"{ShortCachingKey(hashLeagueIds)}-long";

    private async Task<List<Match>> GetAndCachingMatches(GetWaitingMultiSportCriteria criteria, string hashLeagueIds)
    {
        var waitingMatches = await GetMatchesAllSites(criteria);
        var listResultDetail = await ResultDetailByMatchIds(waitingMatches);

        waitingMatches = await matchService.SetNewMatchInfo(waitingMatches, listResultDetail);

        var matches = GetMoreScore(new MultiSportResult { Matches = waitingMatches, ResultDetails = listResultDetail }, criteria.IsLeader);

        return SetToCache(hashLeagueIds, matches);
    }

    private async Task<List<Match>> GetMatchesAllSites(GetWaitingMultiSportCriteria criteria)
    {
        var siteDbServer = appSettingsKey.Value.SiteDbServer;
        var fetchMatchTasks = siteDbServer.Select(s => FetchMatches(s, criteria)).ToList();
        var listMatch = await Task.WhenAll(fetchMatchTasks.ToArray());

        var matchesInAllServers = listMatch.Select(m => m.ToList()).ToList();

        var Matches = matchesInAllServers.SelectMany(m => m).Distinct().ToList();

        for (var i = 1; i < siteDbServer.Count; i++)
        {
            matchService.AddSiteInfo(Matches, matchesInAllServers[i]);
        }

        return Matches;
    }

    private async Task<IEnumerable<Match>> FetchMatches(string server, GetWaitingMultiSportCriteria criteria)
    {
        criteria.DbServer = server;
        return await repository.FetchAsync<Match>(criteria);
    }

    private List<Match> SetToCache(string hashLeagueIds, List<Match> matches)
    {
        var shortCachingKey = ShortCachingKey(hashLeagueIds);
        var longCachingKey = LongCachingKey(hashLeagueIds);

        cacheService.Set(shortCachingKey, matches, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(CacheSettings.Expiration.ShortCallRefreshTimerSeconds)));
        cacheService.Set(longCachingKey, matches, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheSettings.Expiration.LongCallRefreshTimerMinutes)));

        return matches;
    }

    private List<Match> GetMoreScore(MultiSportResult data, bool isLeader)
    {
        var matchesByEachSport = data.Matches.GroupBy(match => match.SportType)
            .Select(sports => new MatchesBySportTypeModel { SportType = sports.Key, Matches = sports.ToList() })
            .ToList();

        var listMatch = new List<Match>();
        foreach (var item in matchesByEachSport)
        {
            item.Matches = matchService.GetScoreForMatchBySportType(item.Matches, data.ResultDetails, item.SportType, isLeader);
            listMatch.AddRange(item.Matches);
        }

        return listMatch;
    }

    private async Task<List<ResultDetail>> ResultDetailByMatchIds(List<Match> matches)
    {
        var listMatchId = string.Join(",", matches.DistinctBy(m => m.MatchId).Select(match => match.MatchId.ToString()).ToArray());
        var resultDetailCriteria = new GetResultDetailCriteria { MatchIdList = listMatchId };
        return (await repository.FetchAsync<ResultDetail>(resultDetailCriteria)).ToList();
    }
}