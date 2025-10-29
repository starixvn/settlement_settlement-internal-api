namespace Settlement.JackTime.Models;

public class NotificationsModels
{
    public ulong Offset { get; set; }

    public List<BetBuilderOddsNotification> Notifications { get; set; } = [];
}

public class BetBuilderOddsNotification
{
    public string MatchId { get; set; }

    public string BetId { get; set; }

    public decimal Odds { get; set; }
}