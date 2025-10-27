namespace Settlement.JackTime.Models;

public class NotificationsModels
{
    public ulong Offset { get; set; }

    public List<Leg> Legs { get; set; } = new List<Leg>();
}

public class Leg
{
    public int MatchId { get; set; }

    public int Bettype { get; set; }

    public decimal Hdp { get; set; }

    public string BetTeam { get; set; }

    public int WayId { get; set; }

    public decimal Odd { get; set; }
}