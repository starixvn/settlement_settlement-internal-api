using Settlement.JackTime.Models;
using Settlement.JackTime.Requests;
using Settlement.JackTime.Services;

namespace Settlement.JackTime.Endpoints;

public class VoidLegsEndpoint(VoidLegsServices voidLegsServices) : IEndpoint<IResult, VoidLegRequest>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPost("api/jacktime-cash-out/void-legs",
             async (VoidLegRequest request) => await HandleAsync(request))
            .Produces<Results<string>>()
            .WithTags("JackTimeEndpoints");
    }

    public async Task<IResult> HandleAsync(VoidLegRequest request)
    {
        var balls = await voidLegsServices.VoidLegsAsync(request);

        return Results.Ok(balls);
    }
}