namespace Settlement.EventManagement.Models;

public class SiteMatch
{
    public int? IsCalculatingSingle { get; set; }

    public int? IsCalculatingMP { get; set; }

    public bool IsExistMarkedTicket { get; set; }

    public bool IsExistRunningTicket { get; set; }

    public bool IsExistEarlyBetTypes { get; set; }

    public bool IsRunningEarlyBetTypes { get; set; }

    public bool IsWaitingEarlyBetTypes { get; set; }
}