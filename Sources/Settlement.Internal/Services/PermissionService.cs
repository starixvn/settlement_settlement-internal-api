using Fanex.Caching;
using Fanex.NAP.Client.Configuration;
using Fanex.NAP.Client.Helpers.Business;
using Fanex.NAP.Client.Helpers.Web;
using Fanex.NAP.Client.Services;
using Settlement.Internal.Constants;
using Settlement.Internal.Models;

namespace Settlement.Internal.Services;

public abstract class PermissionService(
    INapSession session,
    IUserService userService,
    ICacheService cacheService,
    INapSettings siteSettings)
{
    protected abstract Permission Permission { get; }

    protected bool HasPermission(string functionKey, Constant.Policy policy)
    {
        return session.UserId > 0 && userService.IsHavingPermission(
            session.UserId,
            siteSettings.SiteId,
            functionKey,
            policy);
    }

    public Permission GetPermission(string cacheKey)
    {
        var key = $"{cacheKey}-{session.UserId}";
        return cacheService.GetOrSet(
            key,
            () => Permission,
            new CacheItemOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(2)));
    }
}

public class PermissionServiceImp(
    INapSession session,
    IUserService userService,
    ICacheService cacheService,
    INapSettings siteSettings)
    : PermissionService(session, userService, cacheService, siteSettings)
{
    protected override Permission Permission => new()
    {
        CanCalculate = HasPermission(FunctionKeys.SportsbookCalculate, Constant.Policy.Host),
        CanMarkPending = HasPermission(FunctionKeys.SportsbookMarkPendingMatch, Constant.Policy.Host),
        CanMarkUnDecided = HasPermission(FunctionKeys.SportsbookMarkUndecidedMatch, Constant.Policy.Host),
        CanUncalculate = HasPermission(FunctionKeys.SportsbookUncalculate, Constant.Policy.Host),
        CanUncalculateInstant = HasPermission(FunctionKeys.SportsbookUncalculateInstant, Constant.Policy.Host),
        CanRefund = HasPermission(FunctionKeys.SportsbookRefund, Constant.Policy.Host),
        CanViewSettlementLog = HasPermission(FunctionKeys.ViewLogSettlement, Constant.Policy.Read),
        CanAutoPayout = HasPermission(FunctionKeys.AutoPayoutSetting, Constant.Policy.Host),
        IsLeader = HasPermission(FunctionKeys.LeaderCalculate, Constant.Policy.Host),
        CanChangeScoreSource = HasPermission(FunctionKeys.ScoreSource, Constant.Policy.Host),
        CanMoveTickets = HasPermission(FunctionKeys.CricketMoveTickets, Constant.Policy.Host),
        IsTrader = HasPermission(FunctionKeys.TraderSaveScore, Constant.Policy.Host),
        AutoSettlement = HasPermission(FunctionKeys.AutoSettlement, Constant.Policy.Host)
    };
}