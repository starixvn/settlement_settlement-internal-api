namespace Settlement.Internal.Models;

public class AppSettingsKey
{
    public List<string> SiteDbServer { get; set; }

    public bool EnableLogRequest { get; set; }

    public ThirdPartySettingKeys ThirdPartyUri { get; set; }
}