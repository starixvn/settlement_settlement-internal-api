using Fanex.RemotingRef;

namespace Settlement.Internal.Services;

public interface IRefDataService
{
    string GetLeagueName(object leagueId);
}

public class RefDataService(IRemotingRefService remotingRefService) : IRefDataService
{
    private const string LanguageCode = "en";

    public string GetLeagueName(object leagueId)
    {
        return remotingRefService.GetLeagueName(leagueId.ToString(), LanguageCode, string.Empty);
    }
}