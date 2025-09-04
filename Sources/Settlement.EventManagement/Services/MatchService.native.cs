using Settlement.EventManagement.Constants;
using Settlement.EventManagement.Models;
using Settlement.Internal.Constants;

namespace Settlement.EventManagement.Services;

public interface MatchService
{
    List<Match> GetScoreForMatchBySportType(List<Match> matches, List<ResultDetail> resultDetails, int sportType, bool isLeader = false);

    Task<List<Match>> SetNewMatchInfo(List<Match> matches, List<ResultDetail> listResultDetail);

    void AddSiteInfo(List<Match> baseMatches, List<Match> siteMatches);
}

public class MatchServiceImp(
    NewInfoMatchService newInfoMatchService,
    CompareMatchService compareMatchService,
    MatchResultDetailService matchResultDetailService) : MatchService
{
    public async Task<List<Match>> SetNewMatchInfo(List<Match> matches, List<ResultDetail> listResultDetail)
    {
        foreach (var match in matches)
        {
            var listSourceByMatchId = listResultDetail.Where(m => m.MatchId == match.MatchId && m.SourceId != SourceType.Normal).ToList();
            var currentResultDetail = listResultDetail.Where(m => m.MatchId == match.MatchId && m.SourceId == SourceType.Normal).ToList();

            match.NewEventStatus = newInfoMatchService.NewEventStatus(match, currentResultDetail);
            match.EventStatusColor = newInfoMatchService.EventStatusColor(match);
            match.EarlySettleStatus = newInfoMatchService.EarlySettlementStatus(match);
            match.ConfirmHalfTimeScore = newInfoMatchService.ConfirmScore(listSourceByMatchId, ResourceType.Halftime);
            match.ConfirmFullTimeScore = newInfoMatchService.ConfirmScore(listSourceByMatchId, ResourceType.Final);
        }

        return await newInfoMatchService.GetAutoSetting(matches);
    }

    public void AddSiteInfo(List<Match> baseMatches, List<Match> siteMatches)
    {
        foreach (var match in baseMatches)
        {
            var matchSiteInfo = compareMatchService.GetDifferentMatchSiteInfo(match, siteMatches);
            if (matchSiteInfo != null)
            {
                match.SiteMatch.Add(matchSiteInfo);
            }
        }
    }

    public List<Match> GetScoreForMatchBySportType(List<Match> matches, List<ResultDetail> resultDetails, int sportType, bool isLeader = false)
    {
        var sportTypeMappings = new Dictionary<int, Func<List<Match>, List<ResultDetail>, bool, List<Match>>>
        {
            { SportType.Tennis, matchResultDetailService.TennisResultDetail },
            { SportType.BasketBall, matchResultDetailService.BasketballResultDetail },
            { SportType.ESports, matchResultDetailService.ESportsResultDetail },
            { SportType.Badminton, matchResultDetailService.BadmintonResultDetail },
            { SportType.Cricket, matchResultDetailService.CricketResultDetail },
            { SportType.BeachVolleyball, matchResultDetailService.BeachVolleyballResultDetail },
            { SportType.VolleyBall, matchResultDetailService.VolleyballResultDetail },
            { SportType.Bandy, matchResultDetailService.BandyResultDetail },
            { SportType.TableTennis, matchResultDetailService.TableTennisResultDetail },
            { SportType.BaseBall, matchResultDetailService.BaseballResultDetail }
        };

        if (sportTypeMappings.TryGetValue(sportType, out var resultDetailFunc))
        {
            return resultDetailFunc(matches, resultDetails, isLeader);
        }

        return LoadExternalResults(matches, resultDetails, sportType);
    }

    private List<Match> LoadExternalResults(List<Match> matches, List<ResultDetail> resultDetails, int sportType)
    {
        if (Sports.FullTimeSports.Contains(sportType))
        {
            return matchResultDetailService.OtherSportsResultDetail(matches, resultDetails, false);
        }

        if (Sports.HalfTimeFullTimeSports.Contains(sportType))
        {
            return matchResultDetailService.OtherSportsResultDetail(matches, resultDetails, true);
        }

        return matches;
    }
}