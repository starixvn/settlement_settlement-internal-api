using Google.Protobuf;
using Microsoft.Extensions.Options;
using Settlement.Internal.Models;
using Settlement.JackTime.Models;

namespace Settlement.JackTime.Services;

public interface NotificationsServices
{
    Task<Results<NotificationsModels>> GetNotificationsAsync(ulong offset);
}

public class NotificationsServicesImp(
    IOptions<AppSettingsKey> appSettingsKey,
    InvokeExternalAPIServices invokeExternalApiServices,
    GenerateServices generateServices,
    CreateRequestServices createRequestServices) : NotificationsServices
{
    public async Task<Results<NotificationsModels>> GetNotificationsAsync(ulong offset)
    {
        var requestNotification = createRequestServices.CreateNotificationsRequest(offset);
        var requestString = new JsonFormatter(JsonFormatter.Settings.Default).Format(requestNotification);
        var apiUrl = $"{appSettingsKey.Value.ThirdPartyUri.JackTimeNotifications}";

        var response = await invokeExternalApiServices.PostAsync(requestString, apiUrl);
        await LogService.InfoAsync($"JackTime Notifications Response: {response}");
        return await generateServices.GetNotificationsAsync(response);
    }
}