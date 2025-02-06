using Settlement.EventManagement.Services;

namespace Settlement.InternalApi.Config;

public static class EventManagementConfig
{
    public static void AddEventManagementServices(this IServiceCollection services)
    {
        services.AddTransient<MultiSportService, MultiSportServiceImp>();
        services.AddTransient<ScoreTennisService, ScoreTennisServiceImp>();
        services.AddTransient<ExternalScoreService, ExternalScoreServiceImp>();
        services.AddTransient<MatchResultDetailService, MatchResultDetailServiceImp>();
        services.AddTransient<ScoreSetService, ScoreSetServiceImp>();
        services.AddTransient<MatchService, MatchServiceImp>();
        services.AddTransient<CompareMatchService, CompareMatchServiceImp>();
        services.AddTransient<NewInfoMatchService, NewInfoMatchServiceImp>();
        services.AddTransient<SiteMatchService, SiteMatchServiceImp>();
        services.AddTransient<ScoreService, ScoreServiceImp>();
        services.AddTransient<AdjustEventStatusService, AdjustEventStatusServiceImp>();
    }
}