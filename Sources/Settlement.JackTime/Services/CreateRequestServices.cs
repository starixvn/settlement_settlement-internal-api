using Settlement.JackTime.Requests;

namespace Settlement.JackTime.Services;

public interface CreateRequestServices
{
    CommandRequestMessage CreateVoidCommandRequest(VoidBetInfo voidBetInfo);

    CommandRequestMessage CreateUnVoidCommandRequest(VoidBetInfo voidBetInfo);

    NotificationsQueryMessage CreateNotificationsRequest(ulong offset);
}

public class CreateRequestServicesImp : CreateRequestServices
{
    public CommandRequestMessage CreateVoidCommandRequest(VoidBetInfo voidBetInfo)
    {
        var commandRequest = new CommandRequestMessage
        {
            Header = CreateCommandRequestHeader(voidBetInfo),
            Request = new CommandRequest
            {
                VoidLegs = CreateVoidLegsRequest(voidBetInfo)
            }
        };

        foreach (var leg in voidBetInfo.Legs)
        {
            var wayIdentifier = CreateWayIdentifier(leg);
            commandRequest.Request.VoidLegs.Legs.Add(wayIdentifier);
        }

        return commandRequest;
    }

    public CommandRequestMessage CreateUnVoidCommandRequest(VoidBetInfo voidBetInfo)
    {
        var commandRequest = new CommandRequestMessage
        {
            Header = CreateCommandRequestHeader(voidBetInfo),
            Request = new CommandRequest
            {
                UnvoidLegs = CreateUnVoidLegsRequest(voidBetInfo)
            }
        };

        foreach (var leg in voidBetInfo.Legs)
        {
            var wayIdentifier = CreateWayIdentifier(leg);
            commandRequest.Request.UnvoidLegs.Legs.Add(wayIdentifier);
        }

        return commandRequest;
    }

    private static CommandRequestHeader CreateCommandRequestHeader(VoidBetInfo voidBetInfo)
    {
        return new CommandRequestHeader
        {
            TransactionId = new TransactionId
            {
                Id = voidBetInfo.TransactionId
            }
        };
    }

    private static VoidLegsRequest CreateVoidLegsRequest(VoidBetInfo voidBetInfo)
    {
        return new VoidLegsRequest
        {
            Bet = new BetIdentifier
            {
                BetId = new BetId
                {
                    Id = voidBetInfo.TransId
                },
                UserId = new UserId
                {
                    Id = voidBetInfo.CustId.ToString()
                }
            }
        };
    }

    private static UnvoidLegsRequest CreateUnVoidLegsRequest(VoidBetInfo voidBetInfo)
    {
        return new UnvoidLegsRequest
        {
            Bet = new BetIdentifier
            {
                BetId = new BetId
                {
                    Id = voidBetInfo.TransId
                },
                UserId = new UserId
                {
                    Id = voidBetInfo.CustId.ToString()
                }
            }
        };
    }

    private static WayIdentifier CreateWayIdentifier(Leg leg)
    {
        return new WayIdentifier
        {
            WayId = new WayId
            {
                Id = leg.WayId
            },
            Market = new MarketIdentifier
            {
                MatchId = new MatchId
                {
                    Id = leg.MatchId.ToString()
                },
                MarketId = new MarketId
                {
                    Id = leg.MarketId
                }
            }
        };
    }

    public NotificationsQueryMessage CreateNotificationsRequest(ulong offset)
    {
        var notificationsQuery = new NotificationsQuery
        {
            LastConsumedOffset = offset
        };
        var service = new NotificationServiceIndex
        {
            Index = 0
        };

        return new NotificationsQueryMessage
        {
            Service = service,
            Query = notificationsQuery
        };
    }
}