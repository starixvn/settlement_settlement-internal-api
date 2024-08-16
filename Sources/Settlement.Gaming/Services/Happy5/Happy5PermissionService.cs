using Fanex.NAP.Client.Configuration;
using Fanex.NAP.Client.Helpers.Business;
using Fanex.NAP.Client.Helpers.Web;
using Fanex.NAP.Client.Services;
using Settlement.Internal.Constants;
using Settlement.Internal.Models;

namespace Settlement.Gaming.Services.Happy5;

public interface PermissionService
{
    public Permission Permission { get; }
}

public class Happy5PermissionServiceImp(INapSession session, IUserService userService, INapSettings siteSettings)
    : PermissionService
{
    public Permission Permission => new()
    {
        CanItAdmin = HasPermission(FunctionKeys.Happy5ResettleMatchBeforeYesterday, Constant.Policy.Host),
        CanCalculate = HasPermission(FunctionKeys.Happy5Recalculate, Constant.Policy.Host),
        CanRefund = HasPermission(FunctionKeys.Happy5Refund, Constant.Policy.Host),
        CanViewSettlementLog = HasPermission(FunctionKeys.ViewLogSettlement, Constant.Policy.Read)
    };

    private bool HasPermission(string functionKey, Constant.Policy policy)
    {
        return session.UserId > 0 && userService.IsHavingPermission(
            session.UserId,
            siteSettings.SiteId,
            functionKey,
            policy);
    }
}