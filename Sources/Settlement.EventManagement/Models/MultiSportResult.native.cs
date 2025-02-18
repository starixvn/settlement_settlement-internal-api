namespace Settlement.EventManagement.Models;

public class MultiSportResult
{
    public List<Match> Matches { get; set; } = [];

    public List<ResultDetail> ResultDetails { get; set; }
}