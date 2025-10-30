using Microsoft.Extensions.DependencyInjection;
using Settlement.JackTime.Services;

namespace Settlement.JackTime;

public static class JackTimeConfig
{
    public static void JackTimeConfigServices(this IServiceCollection services)
    {
        services.AddTransient<GenerateServices, GenerateServicesImp>();
        services.AddTransient<InvokeExternalAPIServices, InvokeExternalAPIServicesImp>();
        services.AddTransient<NotificationsServices, NotificationsServicesImp>();
        services.AddTransient<BetBuilderOddsNotificationServices, BetBuilderOddsNotificationServicesImp>();
        services.AddTransient<VoidLegsServices, VoidLegsServicesImp>();
        services.AddTransient<CreateRequestServices, CreateRequestServicesImp>();
    }
}