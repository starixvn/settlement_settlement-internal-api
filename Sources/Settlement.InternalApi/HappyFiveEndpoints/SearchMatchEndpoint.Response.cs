using Settlement.Gaming.Models.Happy5;
using Settlement.Internal.Models;

namespace Settlement.InternalApi.HappyFiveEndpoints;

public class SearchMatchEndpointResponse
{
    public Permission Permission { get; set; }

    public List<LeaguesModel> Leagues { get; set; }
}