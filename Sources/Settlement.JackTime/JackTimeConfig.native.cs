using Microsoft.Extensions.DependencyInjection;
using Settlement.JackTime.Services;

namespace Settlement.JackTime;

public static class JackTimeConfig
{
    public static void JackTimeConfigServices(this IServiceCollection services)
    {
        services.AddTransient<GenerateService, GenerateServiceImp>();
        services.AddTransient<InvokeExternalAPIService, InvokeExternalAPIServiceImp>();
        services.AddTransient<NotificationsService, NotificationsServiceImp>();
        services.AddTransient<BetBuilderOddsNotificationService, BetBuilderOddsNotificationServiceImp>();
        services.AddTransient<CommandService, CommandServiceImp>();
        services.AddTransient<VoidLegsService, VoidLegsServiceImp>();
        services.AddTransient<CreateRequestService, CreateRequestServiceImp>();
    }
}