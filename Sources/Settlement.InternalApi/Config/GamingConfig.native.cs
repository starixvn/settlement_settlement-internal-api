using Happy5 = Settlement.Gaming.Services.Happy5;

namespace Settlement.WebSites.Configurations;

public static class GamingConfig
{
    public static void GamingConfigServices(this IServiceCollection services)
    {
        services.AddTransient<Happy5.MatchService, Happy5.MatchServiceImp>();
        services.AddTransient<Happy5.BallService, Happy5.BallServiceImp>();
        services.AddTransient<Happy5.SettlementService, Happy5.SettlementServiceImp>();
        services.AddTransient<Happy5.LeagueService, Happy5.LeagueServiceImp>();
        services.AddSingleton<Happy5.PermissionService, Happy5.Happy5PermissionServiceImp>();
    }
}