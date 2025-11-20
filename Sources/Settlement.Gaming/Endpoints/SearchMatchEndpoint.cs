using Settlement.Gaming.Criteria.Happy5;
using Settlement.Gaming.Models.Happy5;
using Settlement.Gaming.Services.Happy5;

namespace Settlement.Gaming.Endpoints;

public class SearchMatchEndpoint(
    IMapper mapper,
    LeagueService leagueService)
    : IEndpoint<IResult, SearchMatchEndpointRequest>

{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPost("api/happy5/search",
            async (SearchMatchEndpointRequest request) => await HandleAsync(request))
            .Produces<List<LeaguesModel>>()
            .WithTags("HappyFiveEndpoints");
    }

    public async Task<IResult> HandleAsync(SearchMatchEndpointRequest request)
    {
        var criteria = mapper.Map<GetMatchesCriteria>(request);
        var leagues = await leagueService.Search(criteria);

        return Results.Ok(leagues);
    }
}