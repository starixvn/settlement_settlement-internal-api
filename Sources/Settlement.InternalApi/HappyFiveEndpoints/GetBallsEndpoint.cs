using MinimalApi.Endpoint;
using Settlement.Gaming.Models;
using Settlement.Gaming.Requests;
using Settlement.Gaming.Services.Happy5;

namespace Settlement.InternalApi.HappyFiveEndpoints;

public class GetBallsEndpoint(BallService ballService) : IEndpoint<IResult, GetBallsRequest>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("api/happy5/get-ball/{matchId}",
            async (int matchId) => await HandleAsync(new GetBallsRequest(matchId)))
            .Produces<List<BallModel>>()
            .WithTags("HappyFiveEndpoints");
    }

    public async Task<IResult> HandleAsync(GetBallsRequest request)
    {
        var balls = await ballService.GetBallsByMatchId(request.MatchId);

        return Results.Ok(balls);
    }
}