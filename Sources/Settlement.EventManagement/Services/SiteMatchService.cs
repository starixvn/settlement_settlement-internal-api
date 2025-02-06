using Settlement.EventManagement.Constants;

namespace Settlement.EventManagement.Services;

using Models;

public interface SiteMatchService
{
    bool IsExistMarkedTickets(Match match);

    string GetBackgroundCompletedStatus(Match match);

    string EarlySettlementStatus(Match match);

    bool IsExistsConfirmedEarly(Match match);
}

public class SiteMatchServiceImp : SiteMatchService
{
    public bool IsExistMarkedTickets(Match match)
    {
        var isExistMarkedTicketOnSite = match.SiteMatch.Exists(m => m.IsExistMarkedTicket);
        return match.IsExistMarkedTicket || isExistMarkedTicketOnSite;
    }

    public bool IsExistRunningTickets(Match match)
    {
        var isExistRunningTicketOnSite = match.SiteMatch.Exists(m => m.IsExistRunningTicket);
        return match.IsExistRunningTicket || isExistRunningTicketOnSite;
    }

    public bool IsExistsConfirmedEarly(Match match)
    {
        var isExistsConfirmedEarly = match.SiteMatch.Exists(m => m.IsExistsConfirmedEarly);
        return match.IsExistsConfirmedEarly || isExistsConfirmedEarly;
    }

    public string GetBackgroundCompletedStatus(Match match)
    {
        return IsOnlyMarkedTicket(match)
            ? Background.WhiteBlinking
            : GetBackgroundRunningTicket(match);
    }

    public string EarlySettlementStatus(Match match)
    {
        if (IsExistedRunningEarlyBetTypes(match))
        {
            return TicketStatus.Running;
        }

        return IsExistedWaitingEarlyBetTypes(match) ? TicketStatus.Waiting : TicketStatus.Completed;
    }

    private static bool IsExistedWaitingEarlyBetTypes(Match match)
    {
        return match.IsWaitingEarlyBetTypes || match.SiteMatch.Exists(m => m.IsWaitingEarlyBetTypes);
    }

    private static bool IsExistedRunningEarlyBetTypes(Match match)
    {
        return match.IsRunningEarlyBetTypes || match.SiteMatch.Exists(m => m.IsRunningEarlyBetTypes);
    }

    private string GetBackgroundRunningTicket(Match match)
    {
        if (IsExistMarkedTickets(match))
        {
            return Background.PinkBlinking;
        }

        return IsExistRunningTickets(match) ? Background.Pink : string.Empty;
    }

    private bool IsOnlyMarkedTicket(Match match)
    {
        return IsExistMarkedTickets(match) && !IsExistRunningTickets(match);
    }
}