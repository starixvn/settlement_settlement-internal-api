using Settlement.JackTime.Constants;
using Settlement.JackTime.Requests;

namespace Settlement.JackTime.Services;

public interface CreateRequestService
{
    CommandRequestMessage CreateVoidBetCommandRequest(VoidBetInfo voidBetInfo);

    CommandRequestMessage CreateVoidCommandRequest(VoidBetInfo voidBetInfo);

    CommandRequestMessage CreateUnVoidCommandRequest(VoidBetInfo voidBetInfo);

    NotificationsQueryMessage CreateNotificationsRequest(ulong offset);
}

public class CreateRequestServiceImp(CommandService commandService) : CreateRequestService
{
    public CommandRequestMessage CreateVoidBetCommandRequest(VoidBetInfo voidBetInfo)
    {
        var commandRequest = new CommandRequestMessage
        {
            Header = commandService.CreateCommandRequestHeader(voidBetInfo.TransId),
            Request = new CommandRequest
            {
                VoidBet = commandService.CreateVoidBetRequest(voidBetInfo)
            }
        };

        return commandRequest;
    }

    public CommandRequestMessage CreateVoidCommandRequest(VoidBetInfo voidBetInfo)
    {
        var commandRequest = new CommandRequestMessage
        {
            Header = commandService.CreateCommandRequestHeader(voidBetInfo.TransactionId),
            Request = new CommandRequest
            {
                VoidLegs = commandService.CreateVoidLegsRequest(voidBetInfo)
            }
        };

        PopulateLegsSelections(voidBetInfo, commandRequest.Request.VoidLegs.BetBuilderSelections, commandRequest.Request.VoidLegs.Legs);

        return commandRequest;
    }

    public CommandRequestMessage CreateUnVoidCommandRequest(VoidBetInfo voidBetInfo)
    {
        var commandRequest = new CommandRequestMessage
        {
            Header = commandService.CreateCommandRequestHeader(voidBetInfo.TransactionId),
            Request = new CommandRequest
            {
                UnvoidLegs = commandService.CreateUnVoidLegsRequest(voidBetInfo)
            }
        };

        PopulateLegsSelections(voidBetInfo, commandRequest.Request.UnvoidLegs.BetBuilderSelections, commandRequest.Request.UnvoidLegs.Legs);

        return commandRequest;
    }

    private void PopulateLegsSelections(
        VoidBetInfo voidBetInfo,
        ICollection<WayIdentifier> betBuilderSelections,
        ICollection<WayIdentifier> legs)
    {
        foreach (var leg in voidBetInfo.Selections)
        {
            var wayIdentifier = commandService.CreateWayIdentifier(leg);
            if (voidBetInfo.Type == VoidLegsType.BuilderSelections)
            {
                betBuilderSelections.Add(wayIdentifier);
            }

            if (voidBetInfo.Type == VoidLegsType.Legs)
            {
                legs.Add(wayIdentifier);
            }
        }
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