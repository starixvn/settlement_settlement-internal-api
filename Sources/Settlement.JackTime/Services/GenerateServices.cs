using Fanex.Logging;
using Settlement.JackTime.Models;

namespace Settlement.JackTime.Services;

public interface GenerateServices
{
    Task<Results<NotificationsModels>> GetNotificationsAsync(string response);
}

public class GenerateServicesImp(NotificationsServices notificationsServices) : GenerateServices
{
    public async Task<Results<NotificationsModels>> GetNotificationsAsync(string response)
    {
        var message = string.Empty;
        NotificationsModels? data = null;
        try
        {
            var commandReplyMessage = NotificationsQueryResultMessage.Parser.ParseJson(response);
            var allLegs = notificationsServices.ExtractLegsFromNotifications(commandReplyMessage.QueryResult.Notifications.Notifications_);

            data = new NotificationsModels
            {
                Offset = commandReplyMessage.QueryResult.Notifications.Offset,
                Notifications = allLegs
            };
        }
        catch (Exception ex)
        {
            await LogService.ErrorAsync($"JackTime GetNotificationsAsync: {ex.Message}");
            message = ex.Message;
        }

        return new Results<NotificationsModels>
        {
            ErrMsg = message,
            Data = data
        };
    }
}