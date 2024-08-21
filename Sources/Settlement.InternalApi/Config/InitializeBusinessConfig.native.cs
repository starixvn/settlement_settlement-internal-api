using Settlement.Business.Services;
using Settlement.Business.Services.HappyFive;

namespace Settlement.InternalApi.Config;

public static class InitializeBusinessConfig
{
    public static void AddInitializeBusinessServices(this IServiceCollection services)
    {
        services.AddSingleton<BatchTicketsService, BatchTicketsServiceImp>();
        services.AddSingleton<CommandService, CommandServiceImp>();
        services.AddSingleton<ProcessService, ProcessServiceImp>();
        services.AddSingleton<MatchService, MatchServiceImp>();
        services.AddSingleton<CustomerService, CustomerServiceImp>();
        services.AddSingleton<ModdsService, ModdsServiceImp>();
        services.AddSingleton<ModdsValueService, ModdsValueServiceImp>();
        services.AddSingleton<PrepareTicketService, PrepareTicketServiceImp>();
        services.AddSingleton<InsertHistoryData, InsertHistoryDataImp>();
        services.AddSingleton<TicketService, TicketServiceImp>();
        services.AddSingleton<WinLimitService, WinLimitServiceImp>();
        services.AddSingleton<WinLostService, WinLostServiceImp>();
        services.AddSingleton<WinLostByModdsService, WinLostByModdsServiceImp>();
        services.AddSingleton<CommissionService, CommissionServiceImp>();
        services.AddSingleton<ProcessService, ProcessServiceImp>();
    }
}