namespace Settlement.InternalApi.HappyFiveEndpoints;

public class SearchMatchEndpointRequest
{
    public string MatchCode { get; set; }

    public string Status { get; set; }

    public DateTimeOffset FromDate { get; set; } = DateTimeOffset.Now.AddDays(-1);

    public DateTimeOffset ToDate { get; set; } = DateTimeOffset.Now;
}