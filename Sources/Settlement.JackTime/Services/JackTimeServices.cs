using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fanex.Logging;
using Google.Protobuf;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Settlement.Internal.Models;
using Settlement.JackTime.Requests;

namespace Settlement.JackTime.Services;

public interface JackTimeServices
{
    Task<Models.NotificationsModels> GetNotificationsAsync(NotificationsRequest request);
}

public class JackTimeServicesImp(
    IOptions<AppSettingsKey> appSettingsKey,
    InvokeExternalAPIServices invokeExternalApiServices) : JackTimeServices
{
    public async Task<Models.NotificationsModels> GetNotificationsAsync(NotificationsRequest request)
    {
        var notificationsQuery = new NotificationsQuery
        {
            LastConsumedOffset = request.Offset
        };
        var service = new NotificationServiceIndex
        {
            Index = 0
        };

        var requestNotification = new NotificationsQueryMessage
        {
            Service = service,
            Query = notificationsQuery
        };

        var requestString = new JsonFormatter(JsonFormatter.Settings.Default).Format(requestNotification);
        var url = $"{appSettingsKey.Value.ThirdPartyUri.JackTimeNotifications}query";

        var response = await invokeExternalApiServices.PostAsync(requestString, url);

        try
        {
            var commandReplyMessage = NotificationsQueryResultMessage.Parser.ParseJson(response);
            if (commandReplyMessage.QueryResult.Notifications.Notifications_.Count > 0)
            {
                Console.WriteLine($"End JackTime BetCore Object {JsonConvert.SerializeObject(commandReplyMessage)}");
                await LogService.ErrorAsync($"JackTime Response: {JsonConvert.SerializeObject(commandReplyMessage)}");
                return new Models.NotificationsModels
                {
                    Offset = commandReplyMessage.QueryResult.Notifications.Offset
                };
            }
        }
        catch (Exception ex)
        {
            await LogService.ErrorAsync($"JackTime Response: {ex.Message}");
        }

        return new Models.NotificationsModels();
    }
}