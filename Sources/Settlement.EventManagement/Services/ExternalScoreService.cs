using Settlement.EventManagement.Models;
using Settlement.EventManagement.Services.MultiSports;
using Settlement.Internal.Constants;

namespace Settlement.EventManagement.Services;

public interface ExternalScoreService
{
    bool IsInconsistentFullTimeScore(int matchId, string userConfirmation, List<ResultDetail> resultDetails);
    bool IsInconsistentFullTimeScore(Match match, IEnumerable<ResultDetail> resultDetails);
    bool IsInconsistentHalftimeScore(Match match, IEnumerable<ResultDetail> resultDetails);
}

public class ExternalScoreServiceImp(MuayThaiHelperService muayThaiHelperService) : ExternalScoreService
{
    public bool IsInconsistentFullTimeScore(int matchId, string userConfirmation, List<ResultDetail> resultDetails)
    {
        var externalResultDetails = GetExternalResults(matchId, resultDetails);
        var finalExternalResult = FindFinalResult(externalResultDetails);

        if (finalExternalResult == null)
        {
            return false;
        }

        var currentResult = FindCurrentResult(matchId, resultDetails, finalExternalResult.ResourceId);
        return AreScoresDifferent(currentResult, finalExternalResult, userConfirmation);
    }

    public bool IsInconsistentFullTimeScore(Match match, IEnumerable<ResultDetail> resultDetails)
    {
        var externalResultDetails = GetExternalResults(match.MatchId, resultDetails);
        var finalExternalResult = FindFinalResult(externalResultDetails);

        if (finalExternalResult == null)
        {
            return false;
        }

        if (HasNoFinalScores(match))
        {
            return IsMatchSuspended(match.SuspenderFullTime);
        }

        var mappedExternalResult = MapExternalScoreForSport(match.SportType, finalExternalResult);
        return AreScoresInconsistent(match.FinalHomeScore, match.FinalAwayScore, mappedExternalResult);
    }

    public bool IsInconsistentHalftimeScore(Match match, IEnumerable<ResultDetail> resultDetails)
    {
        var externalResultDetails = GetExternalResults(match.MatchId, resultDetails);
        var halftimeExternalResult = FindHalftimeResult(externalResultDetails);

        if (halftimeExternalResult == null)
        {
            return false;
        }

        if (HasNoHalftimeScores(match))
        {
            return IsMatchSuspended(match.SuspenderHalfTime);
        }

        return AreScoresInconsistent(match.HalfTimeHomeScore, match.HalfTimeAwayScore, halftimeExternalResult);
    }

    private static ResultDetail FindFinalResult(List<ResultDetail> externalResults)
    {
        return externalResults.Find(result => result.ResourceId == ResourceType.Final);
    }

    private static ResultDetail FindHalftimeResult(List<ResultDetail> externalResults)
    {
        return externalResults.Find(result => result.ResourceId == ResourceType.Halftime);
    }

    private static ResultDetail FindCurrentResult(int matchId, List<ResultDetail> resultDetails, string resourceId)
    {
        return resultDetails.Find(result =>
            result.MatchId == matchId &&
            result.SourceId == SourceType.Normal &&
            result.ResourceId == resourceId);
    }

    private ResultDetail MapExternalScoreForSport(int sportType, ResultDetail externalResult)
    {
        if (sportType == SportType.MuayThai)
        {
            return MapMuayThaiScore(externalResult);
        }

        return externalResult;
    }

    private ResultDetail MapMuayThaiScore(ResultDetail externalResult)
    {
        if (int.TryParse(externalResult.HomeValue, out var homeValue) &&
            int.TryParse(externalResult.AwayValue, out var awayValue))
        {
            var (mappedHome, mappedAway) = muayThaiHelperService.MapSboToSettle(homeValue, awayValue);
            externalResult.HomeValue = mappedHome;
            externalResult.AwayValue = mappedAway;
        }

        return externalResult;
    }

    private static bool AreScoresDifferent(ResultDetail? currentResult, ResultDetail externalResult, string userConfirmation)
    {
        if (currentResult == null)
        {
            return true;
        }

        if (AreScoresEqual(currentResult, externalResult))
        {
            return false;
        }

        var hasValidScores = HasValidScores(currentResult);
        var hasUserConfirmation = !string.Equals(userConfirmation, "-");

        return hasValidScores || hasUserConfirmation;
    }

    private static bool AreScoresEqual(ResultDetail currentResult, ResultDetail externalResult)
    {
        var homeScoresMatch = string.Equals(currentResult.HomeValue, externalResult.HomeValue);
        var awayScoresMatch = string.Equals(currentResult.AwayValue, externalResult.AwayValue);

        return homeScoresMatch && awayScoresMatch;
    }

    private static bool HasValidScores(ResultDetail result)
    {
        return !string.IsNullOrWhiteSpace(result.HomeValue) &&
               !string.IsNullOrWhiteSpace(result.AwayValue);
    }

    private static bool AreScoresInconsistent(int? homeScore, int? awayScore, ResultDetail externalResult)
    {
        var homeScoresMatch = string.Equals(homeScore?.ToString(), externalResult.HomeValue);
        var awayScoresMatch = string.Equals(awayScore?.ToString(), externalResult.AwayValue);

        return !homeScoresMatch || !awayScoresMatch;
    }

    private static bool HasNoFinalScores(Match match)
    {
        return match.FinalHomeScore == null && match.FinalAwayScore == null;
    }

    private static bool HasNoHalftimeScores(Match match)
    {
        return match.HalfTimeHomeScore == null && match.HalfTimeAwayScore == null;
    }

    private static bool IsMatchSuspended(string suspenderValue)
    {
        return !string.Equals(suspenderValue, "-");
    }

    private static List<ResultDetail> GetExternalResults(int matchId, IEnumerable<ResultDetail> resultDetails)
    {
        return resultDetails
            .Where(result =>
                result.MatchId == matchId &&
                result.SourceId != SourceType.Normal &&
                HasValidScores(result)).ToList();
    }
}