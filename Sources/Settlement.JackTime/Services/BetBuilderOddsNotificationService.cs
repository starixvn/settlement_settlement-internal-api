using Google.Protobuf.Collections;

namespace Settlement.JackTime.Services;

public interface BetBuilderOddsNotificationService
{
    List<Models.BetBuilderOddsNotification> ExtractOddsFromNotifications(RepeatedField<Notification> notifications);
}

public class BetBuilderOddsNotificationServiceImp : BetBuilderOddsNotificationService
{
    public List<Models.BetBuilderOddsNotification> ExtractOddsFromNotifications(RepeatedField<Notification> notifications)
    {
        return [.. notifications
            .Where(notification => notification.BetBuilderOdds != null)
            .Select(notification => notification.BetBuilderOdds)
            .Select(betBuilderOdd => new Models.BetBuilderOddsNotification
            {
                MatchId = betBuilderOdd.MatchId.Id,
                TransId = betBuilderOdd.Bet.BetId.Id,
                Odds = Math.Truncate(Convert.ToDecimal(betBuilderOdd.Odds) * 100) / 100
            })];
    }
}