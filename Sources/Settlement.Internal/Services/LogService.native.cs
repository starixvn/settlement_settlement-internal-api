using Fanex.Data.Repository;
using Settlement.Internal.Criteria;
using Log = Fanex.Logging.LogService;

namespace Settlement.Internal.Services;

public interface LogService
{
    Task WriteLog(int matchId, string message, int actionId, int userId, string ip);

    Task WriteLog(int matchId, string message, int userId);
}

public class LogServiceImp(IDynamicRepository repository) : LogService
{
    public async Task WriteLog(int matchId, string message, int actionId, int userId, string ip)
    {
        var criteria = new LogCriteria
        {
            MatchId = matchId,
            Remark = message,
            UserId = userId,
            ActionId = actionId,
            Ip = ip
        };
        await repository.ExecuteAsync(criteria);
    }

    public async Task WriteLog(int matchId, string message, int userId)
    {
        try
        {
            var criteria = new LogDirectlyCriteria
            {
                Extend = matchId,
                Doing = message,
                UserId = userId
            };

            await repository.ExecuteAsync(criteria);
        }
        catch (Exception e)
        {
            await Log.ErrorAsync("Settlement.WebSites(Write Log Directly): ", e);
        }
    }
}