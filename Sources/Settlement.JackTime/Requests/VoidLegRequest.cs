namespace Settlement.JackTime.Requests;

public class VoidLegRequest
{
    public List<VoidBetInfo> VoidBetInfos { get; set; } = [];
}

public class VoidBetInfo
{
    public string TransactionId { get; set; } = string.Empty;

    public string TransId { get; set; } = string.Empty;

    public string MatchId { get; set; } = string.Empty;

    public long CustId { get; set; }

    public byte Type { get; set; }

    public List<Selection> Selections { get; set; } = [];
}

public class Selection
{
    public string WayId { get; set; } = string.Empty;

    public int MatchId { get; set; }

    public string MarketId { get; set; } = string.Empty;
}