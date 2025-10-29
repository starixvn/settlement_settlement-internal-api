using Google.Protobuf;
using Microsoft.Extensions.Options;
using Settlement.Internal.Models;
using Settlement.JackTime.Models;

namespace Settlement.JackTime.Services;

public interface JackTimeServices
{
    Task<Results<NotificationsModels>> GetNotificationsAsync(ulong offset);
}

public class JackTimeServicesImp(
    IOptions<AppSettingsKey> appSettingsKey,
    InvokeExternalAPIServices invokeExternalApiServices,
    GenerateServices generateServices) : JackTimeServices
{
    public async Task<Results<NotificationsModels>> GetNotificationsAsync(ulong offset)
    {
        var notificationsQuery = new NotificationsQuery
        {
            LastConsumedOffset = offset
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
        return await generateServices.GetNotificationsAsync(response);
    }
}