using Microsoft.Extensions.Options;
using Settlement.Internal.Models;
using Settlement.JackTime.Constants;
using Settlement.JackTime.Models;

namespace Settlement.JackTime.Services;

public interface NotificationsService
{
    Task<Results<NotificationsModels>> GetNotificationsAsync(ulong offset);
}

public class NotificationsServiceImp(
    IOptions<AppSettingsKey> appSettingsKey,
    InvokeExternalAPIService invokeExternalApiServices,
    GenerateService generateServices,
    CreateRequestService createRequestServices) : NotificationsService
{
    public async Task<Results<NotificationsModels>> GetNotificationsAsync(ulong offset)
    {
        if (string.IsNullOrEmpty(appSettingsKey.Value.ThirdPartyUri.JackTimeNotifications))
        {
            return new Results<NotificationsModels>
            {
                ErrMsg = Result.Success
            };
        }

        var requestNotification = createRequestServices.CreateNotificationsRequest(offset);
        var requestString = new JsonFormatter(JsonFormatter.Settings.Default).Format(requestNotification);
        var apiUrl = $"{appSettingsKey.Value.ThirdPartyUri.JackTimeNotifications}";

        var response = await invokeExternalApiServices.PostAsync(requestString, apiUrl);
        return await generateServices.GetNotificationsAsync(response);
    }
}