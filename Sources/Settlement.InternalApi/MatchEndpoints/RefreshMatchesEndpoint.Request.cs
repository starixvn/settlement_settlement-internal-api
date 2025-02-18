namespace Settlement.InternalApi.MatchEndpoints;

public class RefreshMatchesEndpointRequest
{
    public string LeagueIds { get; set; }

    public string MatchIds { get; set; }

    public bool IsLeader { get; set; }
}