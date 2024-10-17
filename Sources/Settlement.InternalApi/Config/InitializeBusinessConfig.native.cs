using Settlement.Business.Services;
using Settlement.Business.Services.Products;

namespace Settlement.InternalApi.Config;

public static class InitializeBusinessConfig
{
    public static void AddInitializeBusinessServices(this IServiceCollection services)
    {
        services.AddSingleton<HappyFiveProcessService, HappyFiveProcessServiceImp>();
        services.AddSingleton<RacingProcessService, RacingProcessServiceImp>();
        services.AddSingleton<MatchService, MatchServiceImp>();

        services.AddSingleton<SettleService, SettleServiceImp>();

        services.AddSingleton<TicketValueService, TicketValueServiceImp>();
        services.AddSingleton<GenerateSettleTicketService, GenerateSettleTicketServiceImp>();

        services.AddSingleton<CustomerService, CustomerServiceImp>();
        services.AddSingleton<ModdsService, ModdsServiceImp>();
        services.AddSingleton<ModdsValueService, ModdsValueServiceImp>();
        services.AddSingleton<PrepareTicketService, PrepareTicketServiceImp>();
        services.AddSingleton<HistoryData, HistoryDataImp>();
        services.AddSingleton<TicketService, TicketServiceImp>();
        services.AddSingleton<WinLimitService, WinLimitServiceImp>();
        services.AddSingleton<WinLostService, WinLostServiceImp>();
        services.AddSingleton<WinLostByModdsService, WinLostByModdsServiceImp>();
        services.AddSingleton<CommissionService, CommissionServiceImp>();
    }
}