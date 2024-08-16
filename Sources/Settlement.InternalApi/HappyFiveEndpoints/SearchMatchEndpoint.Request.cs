namespace Settlement.InternalApi.HappyFiveEndpoints;

public class SearchMatchEndpointRequest
{
    public string MatchCode { get; set; }

    public string Status { get; set; }

    public DateTime FromDate { get; set; } = DateTime.Now.AddDays(-1);

    public DateTime ToDate { get; set; } = DateTime.Now;
}