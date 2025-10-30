using Settlement.JackTime.Models;
using Settlement.JackTime.Requests;
using Settlement.JackTime.Services;

namespace Settlement.JackTime.Endpoints;

public class UnVoidLegsEndpoint(VoidLegsServices voidLegsServices) : IEndpoint<IResult, VoidLegRequest>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPost("api/jacktime-cash-out/unvoid-legs",
             async (VoidLegRequest request) => await HandleAsync(request))
            .Produces<Results<string>>()
            .WithTags("JackTimeEndpoints");
    }

    public async Task<IResult> HandleAsync(VoidLegRequest request)
    {
        var balls = await voidLegsServices.UnVoidLegsAsync(request);

        return Results.Ok(balls);
    }
}