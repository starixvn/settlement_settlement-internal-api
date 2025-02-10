using Fanex.Caching;
using Fanex.Caching.Redis;
using Fanex.Data.Repository;
using Fanex.Logging;
using Fanex.NAP.Client.Services;
using Fanex.RemotingRef;
using Fanex.Settlement.Converter.Services;
using Fanex.Settlement.ScaleOut.Services;
using Settlement.Internal.Services;
using StackExchange.Redis;
using ILogger = Fanex.Logging.ILogger;

namespace Settlement.InternalApi.Config;

public static class InitializeConfig
{
    public static IServiceCollection AddInitialServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddCoreServices(configuration);
        services.AddGamingConfigServices();
        services.AddInitializeBusinessServices();

        return services;
    }

    private static void AddCoreServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IUserService, UserService>();
        services.AddSingleton<ISecurityService, SecurityService>();

        services.AddTransient<ILogger, Logger>();
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddTransient<IDynamicRepository, DynamicRepository>();
        services.AddSingleton<IRemotingRefService, RemotingRefService>();
        services.AddTransient<IRefDataService, RefDataService>();
        services.AddSingleton<XmlConvertService, XmlConvertServiceImp>();
        services.AddTransient<Internal.Services.LogService, LogServiceImp>();

        services.AddTransient<ScaleOutSettleService, ScaleOutSettleServiceImp>();
        services.AddTransient<ScaleOutSettleThirdPartyService, ScaleOutSettleThirdPartyServiceImp>();
        services.AddTransient<GeneralService, GeneralServiceImp>();

        RefDataTypeContainer
            .ConnectTo(new Uri(configuration.GetSection("RemotingRef").Value))
            .FromApplication("Settlement");
        services.Configure<IConfiguration>(configuration);
        services.AddSingleton<IRemotingRefService, RemotingRefService>();
    }

    public static void AddConfigRedisCache(this IServiceCollection service, IConfiguration configuration)
    {
        var redisConfig = configuration.GetSection("AuthSettings:RedisConfig").Value;
        service.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect($"{redisConfig}"));
        service.AddSingleton<ICache>(new RedisCache(new RedisCacheOptions
        {
            ConnectionString = redisConfig,
        }));
        service.AddSingleton<ICacheService>(new CacheService());
    }
}