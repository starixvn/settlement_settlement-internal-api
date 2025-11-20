using Settlement.Internal.EndpointFilters;
using Settlement.JackTime.Models;
using Settlement.JackTime.Requests;
using Settlement.JackTime.Services;

namespace Settlement.JackTime.Endpoints;

public class UnVoidLegsEndpoint(VoidLegsService voidLegsServices) : IEndpoint<IResult, VoidLegRequest>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPost("api/jacktime-cash-out/unvoid-legs",
             async (VoidLegRequest request) => await HandleAsync(request))
            .Produces<Results<string>>()
            .AddEndpointFilter<LogRequestFilter>()
            .WithTags("JackTimeEndpoints");
    }

    public async Task<IResult> HandleAsync(VoidLegRequest request)
    {
        var results = await voidLegsServices.UnVoidLegsAsync(request);

        return Results.Ok(results);
    }
}