using Settlement.Gaming.Models;
using Settlement.Internal.Models;

namespace Settlement.Gaming.Requests;

public class NumberGameSettlementRequest
{
    public int MatchId { get; set; }

    public int UserId { get; set; }

    public List<BallModel> Balls { get; set; }

    public string Action { get; set; }

    public Permission Permission { get; set; }
}