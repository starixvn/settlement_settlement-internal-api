namespace Settlement.Gaming.Requests;

public class GetBallsRequest(int matchId)
{
    public int MatchId { get; set; } = matchId;
}