using Settlement.JackTime.Models;
using Settlement.JackTime.Requests;
using Settlement.JackTime.Services;

namespace Settlement.JackTime.Endpoints;

public class VoidBetEndpoint(VoidLegsServices voidLegsServices) : IEndpoint<IResult, VoidBetInfoRequest>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPost("api/jacktime-cash-out/void-bet",
             async (VoidBetInfoRequest request) => await HandleAsync(request))
            .Produces<Results<string>>()
            .WithTags("JackTimeEndpoints");
    }

    public async Task<IResult> HandleAsync(VoidBetInfoRequest request)
    {
        var balls = await voidLegsServices.VoidBetAsync(request);

        return Results.Ok(balls);
    }
}