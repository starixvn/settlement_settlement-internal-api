using MinimalApi.Endpoint;
using Settlement.Gaming.Models;
using Settlement.Gaming.Requests;
using Settlement.Gaming.Services.Happy5;
using Settlement.InternalApi.EndpointFilters;

namespace Settlement.InternalApi.HappyFiveEndpoints;

public class SettleEndpoint(SettleService settlementService) : IEndpoint<IResult, HappyFiveRequest>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPost("api/happy5/settle",
                async (HappyFiveRequest request) => await HandleAsync(request))
            .Produces<List<Result>>()
            .AddEndpointFilter<LogRequestFilter>()
            .WithTags("HappyFiveEndpoints");
    }

    public async Task<IResult> HandleAsync(HappyFiveRequest request)
    {
        var result = await settlementService.Settle(request);

        return Results.Ok(result);
    }
}