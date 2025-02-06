using Settlement.EventManagement.Criteria;

namespace Settlement.EventManagement.Services;

using Constants;
using Models;
using Settlement.Internal.Constants;
using System.Threading.Tasks;

public interface NewInfoMatchService
{
    string NewEventStatus(Match match, IEnumerable<ResultDetail> resultDetails = null);

    string EventStatusColor(Match match);

    string EarlySettlementStatus(Match match);

    string ConfirmScore(List<ResultDetail> listResultDetail, string resource);

    bool IsExistsConfirmedEarly(Match match);

    Task<List<Match>> GetAutoSetting(List<Match> matches);
}

public class NewInfoMatchServiceImp(
    SiteMatchService siteMatchService,
    AdjustEventStatusService adjustEventStatusService,
    IDynamicRepository repository)
    : NewInfoMatchService
{
    public string EventStatusColor(Match match)
    {
        if (match.EventStatus.IgnoreCaseEquals(EventStatus.Postponed))
        {
            return siteMatchService.IsExistMarkedTickets(match)
                ? Background.YellowBlinking
                : Background.Yellow;
        }

        if (match.EventStatus.IgnoreCaseEquals(EventStatus.Completed))
        {
            return siteMatchService.GetBackgroundCompletedStatus(match);
        }

        if (match.Remark.IsPending)
        {
            return siteMatchService.IsExistMarkedTickets(match)
                ? Background.BrightYellowBlinking
                : Background.BrightYellow;
        }

        return siteMatchService.IsExistMarkedTickets(match) ? Background.WhiteBlinking : string.Empty;
    }

    public string NewEventStatus(Match match, IEnumerable<ResultDetail> resultDetails = null)
    {
        var eventStatus = SportGroup.SupportSettleDetermine.Contains(match.SportType)
            ? adjustEventStatusService.EventStatusAbandoned(match.EventStatus, resultDetails, match.SportType)
            : match.EventStatus;

        return CheckProcessingStatus(eventStatus, match);
    }

    private static string CheckProcessingStatus(string eventStatus, Match match)
    {
        if (match.IsCalculatingSingle != CalculateStatus.IsWaiting && match.IsCalculatingSingle != CalculateStatus.IsCalculating)
        {
            return eventStatus;
        }

        return match.IsCalculatingSingle == CalculateStatus.IsWaiting ? EventStatus.Waiting : EventStatus.Calculating;
    }

    public string EarlySettlementStatus(Match match)
    {
        return siteMatchService.EarlySettlementStatus(match);
    }

    public string ConfirmScore(List<ResultDetail> listResultDetail, string resource)
    {
        var resultDetail = listResultDetail.Find(result => result.ResourceId == resource
                                                           && !string.IsNullOrWhiteSpace(result.AwayValue)
                                                           && !string.IsNullOrWhiteSpace(result.HomeValue));

        return resultDetail != null
            ? $"({resultDetail.SourceName}: {resultDetail.HomeValue}-{resultDetail.AwayValue})"
            : string.Empty;
    }

    public bool IsExistsConfirmedEarly(Match match)
    {
        return siteMatchService.IsExistsConfirmedEarly(match);
    }

    public async Task<List<Match>> GetAutoSetting(List<Match> matches)
    {
        var matchStatuses = (await GetMatchStatus(matches)).ToList();
        return matches.Join(matchStatuses, m => m.MatchId, s => s.MatchId, (m, s) =>
        {
            m.AutoPayoutStatus = s.AutoPayoutStatus;
            m.AutoInstantStatus = s.InstantStatus;
            return m;
        }).ToList();
    }

    private async Task<IEnumerable<MatchStatusModel>> GetMatchStatus(IEnumerable<Match> matches)
    {
        var matchIds = string.Join(',', matches.Select(m => m.MatchId));
        var criteria = new GetAutoStatusCriteria
        {
            MatchIds = matchIds
        };

        return await repository.FetchAsync<MatchStatusModel>(criteria);
    }
}