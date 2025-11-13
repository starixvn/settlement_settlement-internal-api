using Settlement.JackTime.Requests;

namespace Settlement.JackTime.Services;

public interface CommandService
{
    CommandRequestHeader CreateCommandRequestHeader(string transactionId);

    VoidBetRequest CreateVoidBetRequest(VoidBetInfo voidBetInfo);

    VoidLegsRequest CreateVoidLegsRequest(VoidBetInfo voidBetInfo);

    UnvoidLegsRequest CreateUnVoidLegsRequest(VoidBetInfo voidBetInfo);

    WayIdentifier CreateWayIdentifier(Selection selection);
}

public class CommandServiceImp : CommandService
{
    public CommandRequestHeader CreateCommandRequestHeader(string transactionId)
    {
        return new CommandRequestHeader
        {
            TransactionId = new TransactionId
            {
                Id = transactionId
            }
        };
    }

    public VoidBetRequest CreateVoidBetRequest(VoidBetInfo voidBetInfo)
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

    public VoidLegsRequest CreateVoidLegsRequest(VoidBetInfo voidBetInfo)
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

    public UnvoidLegsRequest CreateUnVoidLegsRequest(VoidBetInfo voidBetInfo)
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

    public WayIdentifier CreateWayIdentifier(Selection selection)
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
}

