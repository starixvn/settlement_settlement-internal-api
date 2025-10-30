using Google.Protobuf.Collections;

namespace Settlement.JackTime.Services;

public interface BetBuilderOddsNotificationServices
{
    List<Models.BetBuilderOddsNotification> ExtractOddsFromNotifications(RepeatedField<Notification> notifications);
}

public class BetBuilderOddsNotificationServicesImp : BetBuilderOddsNotificationServices
{
    public List<Models.BetBuilderOddsNotification> ExtractOddsFromNotifications(RepeatedField<Notification> notifications)
    {
        return [.. notifications
            .Where(notification => notification.BetBuilderOdds != null)
            .Select(notification => notification.BetBuilderOdds)
            .Select(betBuilderOdd => new Models.BetBuilderOddsNotification
            {
                MatchId = betBuilderOdd.MatchId.Id,
                BetId = betBuilderOdd.Bet.BetId.Id,
                Odds = Convert.ToDecimal(betBuilderOdd.Odds)
            })];
    }
}