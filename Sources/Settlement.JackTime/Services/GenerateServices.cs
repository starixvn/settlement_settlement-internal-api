using Settlement.JackTime.Constants;
using Settlement.JackTime.Models;

namespace Settlement.JackTime.Services;

public interface GenerateServices
{
    Task<Results<NotificationsModels>> GetNotificationsAsync(string response);

    Results<string> CommandReplyMessageAsync(string response);
}

public class GenerateServicesImp(BetBuilderOddsNotificationServices notificationsServices) : GenerateServices
{
    public async Task<Results<NotificationsModels>> GetNotificationsAsync(string response)
    {
        var message = string.Empty;
        NotificationsModels? data = null;
        try
        {
            var commandReplyMessage = NotificationsQueryResultMessage.Parser.ParseJson(response);
            var resultCode = commandReplyMessage.QueryResult.ResultCode.ToString();

            if (IsSuccessResult(resultCode))
            {
                data = ProcessSuccessResponse(commandReplyMessage);
            }

            message = resultCode;
        }
        catch (Exception ex)
        {
            await LogService.ErrorAsync($"JackTime BetBuilderOddsNotification: {ex.Message}");
            message = ex.Message;
        }

        return new Results<NotificationsModels>
        {
            ErrMsg = message,
            Data = data
        };
    }

    public Results<string> CommandReplyMessageAsync(string response)
    {
        var commandReplyMessage = CommandReplyMessage.Parser.ParseJson(response);
        var resultCode = commandReplyMessage.Header.ReplyStatusCode.ToString();

        return new Results<string>
        {
            ErrMsg = resultCode,
        };
    }

    private static bool IsSuccessResult(string resultCode)
    {
        return string.Equals(resultCode, Result.Success, StringComparison.CurrentCultureIgnoreCase);
    }

    private NotificationsModels ProcessSuccessResponse(NotificationsQueryResultMessage commandReplyMessage)
    {
        var notificationsList = notificationsServices.ExtractOddsFromNotifications(commandReplyMessage.QueryResult.Notifications.Notifications_);
        return new NotificationsModels
        {
            Offset = commandReplyMessage.QueryResult.Notifications.Offset,
            Notifications = notificationsList
        };
    }
}