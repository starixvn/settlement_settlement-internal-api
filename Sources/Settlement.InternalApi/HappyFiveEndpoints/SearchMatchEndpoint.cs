using AutoMapper;
using MinimalApi.Endpoint;
using Settlement.Gaming.Criteria.Happy5;
using Settlement.Gaming.Services.Happy5;
using Settlement.InternalApi.EndpointFilters;

namespace Settlement.InternalApi.HappyFiveEndpoints;

public class SearchMatchEndpoint(
    IMapper mapper,
    LeagueService leagueService,
    PermissionService permissionService)
    : IEndpoint<IResult, SearchMatchEndpointRequest>

{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPost("api/happy5/search",
            async (SearchMatchEndpointRequest request) => await HandleAsync(request))
            .Produces<SearchMatchEndpointResponse>()
            .AddEndpointFilter<LogRequestFilter>()
            .WithTags("HappyFiveEndpoints");
    }

    public async Task<IResult> HandleAsync(SearchMatchEndpointRequest request)
    {
        var criteria = mapper.Map<GetMatchesCriteria>(request);
        var leagues = await leagueService.Search(criteria);

        var response = new SearchMatchEndpointResponse
        {
            Permission = permissionService.Permission,
            Leagues = leagues
        };

        return Results.Ok(response);
    }
}