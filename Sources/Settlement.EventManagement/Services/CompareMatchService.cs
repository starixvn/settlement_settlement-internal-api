using Settlement.EventManagement.Models;

namespace Settlement.EventManagement.Services;

public interface CompareMatchService
{
    SiteMatch GetDifferentMatchSiteInfo(Match match, List<Match> siteMatches);
}

public class CompareMatchServiceImp : CompareMatchService
{
    public SiteMatch GetDifferentMatchSiteInfo(Match match, List<Match> siteMatches)
    {
        var siteMatch = siteMatches.Find(m => m.MatchId == match.MatchId);
        if (siteMatch != null && (IsChangedOnTicket(match, siteMatch) || IsChangeOnSpecialBetType(match, siteMatch)))
        {
            return new SiteMatch
            {
                IsCalculatingSingle = siteMatch.IsCalculatingSingle,
                IsCalculatingMP = siteMatch.IsCalculatingMP,
                IsExistMarkedTicket = siteMatch.IsExistMarkedTicket,
                IsExistRunningTicket = siteMatch.IsExistRunningTicket,
                IsExistEarlyBetTypes = siteMatch.IsExistEarlyBetTypes,
                IsRunningEarlyBetTypes = siteMatch.IsRunningEarlyBetTypes,
                IsWaitingEarlyBetTypes = siteMatch.IsWaitingEarlyBetTypes,
                IsExistsConfirmedEarly = siteMatch.IsExistsConfirmedEarly
            };
        }

        return null;
    }

    private static bool IsChangedOnTicket(Match match, Match siteMatch)
    {
        var isExistRunning = match.IsExistRunningTicket != siteMatch.IsExistRunningTicket;

        var isExistMarked = match.IsExistMarkedTicket != siteMatch.IsExistMarkedTicket;

        return match.IsCalculatingSingle != siteMatch.IsCalculatingSingle ||
               match.IsCalculatingMP != siteMatch.IsCalculatingMP
               || isExistMarked
               || isExistRunning;
    }

    private static bool IsChangeOnSpecialBetType(Match match, Match siteMatch)
    {
        return match.IsExistEarlyBetTypes != siteMatch.IsExistEarlyBetTypes
               || match.IsRunningEarlyBetTypes != siteMatch.IsRunningEarlyBetTypes
               || match.IsWaitingEarlyBetTypes != siteMatch.IsWaitingEarlyBetTypes;
    }
}