using Settlement.JackTime.Models;
using Settlement.JackTime.Services;

namespace Settlement.JackTime.Endpoints;

public class NotificationsEndpoint(NotificationsService jackTimeServices) : IEndpoint<IResult, ulong>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("api/jacktime-cash-out/notifications/{offset}",
             async (ulong offset) => await HandleAsync(offset))
            .Produces<Results<NotificationsModels>>()
            .WithTags("JackTimeEndpoints");
    }

    public async Task<IResult> HandleAsync(ulong offset)
    {
        var results = await jackTimeServices.GetNotificationsAsync(offset);

        return Results.Ok(results);
    }
}