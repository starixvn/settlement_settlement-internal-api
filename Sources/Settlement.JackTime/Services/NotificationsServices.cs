using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Settlement.JackTime.Models;

namespace Settlement.JackTime.Services;

public interface NotificationsServices
{
    List<Leg> ExtractLegsFromNotifications(IEnumerable<dynamic> notifications);
}

public class NotificationsServicesImp : NotificationsServices
{
    public List<Leg> ExtractLegsFromNotifications(IEnumerable<dynamic> notifications)
    {
        var allLegs = new List<Leg>();
        foreach (var notification in notifications)
        {
            if (notification.AutoSell?.SellProperties?.LegProbabilities?.Legs != null)
            {
                foreach (var leg in notification.AutoSell.SellProperties.LegProbabilities.Legs)
                {
                    var legModel = new Leg
                    {
                        MatchId = int.Parse(leg.Way.Market.MatchId.Id),
                        BetTeam = leg.Way.WayId.Id,
                        WayId = int.Parse(leg.Way.WayId.Id),
                    };
                    allLegs.Add(legModel);
                }
            }
        }
        return allLegs;
    }
}