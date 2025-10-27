using MinimalApi.Endpoint;
using Settlement.JackTime.Models;
using Settlement.JackTime.Requests;
using Settlement.JackTime.Services;

namespace Settlement.InternalApi.JackTimeEndpoints;

public class NotificationsEndpoint(JackTimeServices jackTimeServices) : IEndpoint<IResult, NotificationsRequest>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPost("api/jacktime/notifications",
             async (NotificationsRequest request) => await HandleAsync(request))
            .Produces<Results<NotificationsModels>>()
            .WithTags("JackTimeEndpoints");
    }

    public async Task<IResult> HandleAsync(NotificationsRequest request)
    {
        var balls = await jackTimeServices.GetNotificationsAsync(request);

        return Results.Ok(balls);
    }
}