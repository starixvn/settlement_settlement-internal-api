using Newtonsoft.Json;
using Settlement.Internal.Services;

namespace Settlement.Gaming.Models.NumberGame;

public class LeaguesModel
{
    [JsonIgnore]
    public IRefDataService RefDataService { get; set; }

    public int LeagueId { get; set; }

    public string LeagueName => RefDataService.GetLeagueName(LeagueId);

    public List<MatchDetailModel> Matches { get; set; }
}