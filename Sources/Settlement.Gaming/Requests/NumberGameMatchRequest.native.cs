namespace Settlement.Gaming.Requests;

public class NumberGameMatchRequest
{
    public string MatchCode { get; set; }

    public string MatchStatus { get; set; }

    public DateTime FromDate { get; set; } = DateTime.Now.AddDays(-1);

    public DateTime ToDate { get; set; } = DateTime.Now;
}