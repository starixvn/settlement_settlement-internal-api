using Settlement.JackTime.Requests;

namespace Settlement.JackTime.Services;

public interface CreateRequestServices
{
    CommandRequestMessage CreateVoidBetCommandRequest(VoidBetInfo voidBetInfo);

    CommandRequestMessage CreateVoidCommandRequest(VoidBetInfo voidBetInfo);

    CommandRequestMessage CreateUnVoidCommandRequest(VoidBetInfo voidBetInfo);

    NotificationsQueryMessage CreateNotificationsRequest(ulong offset);
}

public class CreateRequestServicesImp : CreateRequestServices
{
    public CommandRequestMessage CreateVoidBetCommandRequest(VoidBetInfo voidBetInfo)
    {
        var commandRequest = new CommandRequestMessage
        {
            Header = CreateCommandRequestHeader(voidBetInfo.TransId),
            Request = new CommandRequest
            {
                VoidBet = CreateVoidBetRequest(voidBetInfo)
            }
        };

        return commandRequest;
    }

    public CommandRequestMessage CreateVoidCommandRequest(VoidBetInfo voidBetInfo)
    {
        var commandRequest = new CommandRequestMessage
        {
            Header = CreateCommandRequestHeader(voidBetInfo.TransactionId),
            Request = new CommandRequest
            {
                VoidLegs = CreateVoidLegsRequest(voidBetInfo)
            }
        };

        foreach (var leg in voidBetInfo.Selections)
        {
            var wayIdentifier = CreateWayIdentifier(leg);
            commandRequest.Request.VoidLegs.BetBuilderSelections.Add(wayIdentifier);
        }

        return commandRequest;
    }

    public CommandRequestMessage CreateUnVoidCommandRequest(VoidBetInfo voidBetInfo)
    {
        var commandRequest = new CommandRequestMessage
        {
            Header = CreateCommandRequestHeader(voidBetInfo.TransactionId),
            Request = new CommandRequest
            {
                UnvoidLegs = CreateUnVoidLegsRequest(voidBetInfo)
            }
        };

        foreach (var leg in voidBetInfo.Selections)
        {
            var wayIdentifier = CreateWayIdentifier(leg);
            commandRequest.Request.UnvoidLegs.BetBuilderSelections.Add(wayIdentifier);
        }

        return commandRequest;
    }

    private static CommandRequestHeader CreateCommandRequestHeader(string transactionId)
    {
        return new CommandRequestHeader
        {
            TransactionId = new TransactionId
            {
                Id = transactionId
            }
        };
    }

    private static VoidBetRequest CreateVoidBetRequest(VoidBetInfo voidBetInfo)
    {
        return new VoidBetRequest
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

    private static WayIdentifier CreateWayIdentifier(Selection selection)
    {
        return new WayIdentifier
        {
            WayId = new WayId
            {
                Id = selection.WayId
            },
            Market = new MarketIdentifier
            {
                MatchId = new MatchId
                {
                    Id = selection.MatchId.ToString()
                },
                MarketId = new MarketId
                {
                    Id = selection.MarketId
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