using AutoMapper;
using MinimalApi.Endpoint;
using Settlement.EventManagement.Criteria;
using Settlement.EventManagement.Services;
using Settlement.Gaming.Models.Happy5;

namespace Settlement.InternalApi.MatchEndpoints;

public class RefreshMatchesEndpoint(
    IMapper mapper,
    MultiSportService multiSportService)
    : IEndpoint<IResult, RefreshMatchesEndpointRequest>

{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPost("api/multi-sports/auto-refresh",
            async (RefreshMatchesEndpointRequest request) => await HandleAsync(request))
            .Produces<List<LeaguesModel>>()
            .WithTags("EventManagementEndpoints");
    }

    public async Task<IResult> HandleAsync(RefreshMatchesEndpointRequest request)
    {
        var criteria = mapper.Map<GetWaitingMultiSportCriteria>(request);
        var leagues = await multiSportService.GetWaitingMatches(criteria);

        return Results.Ok(leagues);
    }
}