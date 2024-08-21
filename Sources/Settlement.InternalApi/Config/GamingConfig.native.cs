using Settlement.Gaming.Services.Happy5;

namespace Settlement.InternalApi.Config;

public static class GamingConfig
{
    public static void AddGamingConfigServices(this IServiceCollection services)
    {
        services.AddTransient<MatchService, MatchServiceImp>();
        services.AddTransient<BallService, BallServiceImp>();
        services.AddTransient<SettleService, SettleServiceImp>();
        services.AddTransient<LeagueService, LeagueServiceImp>();
        services.AddTransient<ProcessService, ProcessServiceImp>();
    }
}