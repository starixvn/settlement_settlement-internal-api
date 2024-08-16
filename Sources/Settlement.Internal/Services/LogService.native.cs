using Fanex.Data.Repository;
using Settlement.Internal.Criteria;
using Log = Fanex.Logging.LogService;

namespace Settlement.Internal.Services;

public interface LogService
{
    Task WriteLog(int matchId, string message, int userId);

    Task WriteLog(string message, int actionId, int userId);

    Task WriteLog(InsertLogCriteria criteria);

    Task ActionLog(int matchId, string message, int actionId, int userId);

    Task WriteLog(LogCriteria criteria);
}

public class LogServiceImp(IDynamicRepository repository) : LogService
{
    public async Task ActionLog(int matchId, string message, int actionId, int userId)
    {
        var criteria = new LogCriteria(matchId, message, actionId, userId);
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

    public async Task WriteLog(InsertLogCriteria criteria)
    {
        try
        {
            if (!criteria.IsHedgeMatch)
            {
                await repository.ExecuteAsync(criteria);
            }
        }
        catch (Exception e)
        {
            await Log.ErrorAsync("Settlement.WebSites(Write resettle Log): ", e);
        }
    }

    public async Task WriteLog(string message, int actionId, int userId)
    {
        var criteria = new LogCriteria(message, actionId, userId);
        await repository.ExecuteAsync(criteria);
    }

    public async Task WriteLog(LogCriteria criteria)
    {
        await repository.ExecuteAsync(criteria);
    }
}