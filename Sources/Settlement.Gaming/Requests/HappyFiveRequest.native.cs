using Settlement.Gaming.Models;
using Settlement.Internal.Constants;

namespace Settlement.Gaming.Requests;

public class HappyFiveRequest
{
    public int MatchId { get; set; }

    public int UserId { get; set; }

    public List<BallModel> Balls { get; set; }

    public string Action { get; set; }

    public string Ip { get; set; }

    public string OldResult { get; set; }

    public int ActionId => ActionIdValue(Action);

    private static int ActionIdValue(string actionName) => string.Equals(actionName, ActionType.CalculateAll, StringComparison.OrdinalIgnoreCase)
        ? Constants.ActionId.HappyFiveCalc
        : Constants.ActionId.HappyFiveRefund;
}