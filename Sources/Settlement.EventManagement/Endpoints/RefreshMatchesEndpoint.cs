using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MinimalApi.Endpoint;
using Settlement.EventManagement.Criteria;
using Settlement.EventManagement.Models;
using Settlement.EventManagement.Services;

namespace Settlement.EventManagement.Endpoints;

public class RefreshMatchesEndpoint(
    IMapper mapper,
    MultiSportService multiSportService)
    : IEndpoint<IResult, RefreshMatchesEndpointRequest>

{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPost("api/multi-sports/auto-refresh",
            async (RefreshMatchesEndpointRequest request) => await HandleAsync(request))
            .Produces<List<Match>>()
            .WithTags("EventManagementEndpoints");
    }

    public async Task<IResult> HandleAsync(RefreshMatchesEndpointRequest request)
    {
        var criteria = mapper.Map<GetWaitingMultiSportCriteria>(request);
        var leagues = await multiSportService.GetWaitingMatches(criteria);

        return Results.Ok(leagues);
    }
}