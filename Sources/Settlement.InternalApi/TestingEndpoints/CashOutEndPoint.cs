using MinimalApi.Endpoint;

namespace Settlement.InternalApi.TestingEndpoints;

public class CashOutEndPoint : IEndpoint
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPut("api/cash-out/ResultMarkets", async (HttpContext context) =>
        {
            using var reader = new StreamReader(context.Request.Body);
            var body = await reader.ReadToEndAsync();
            await Fanex.Logging.LogService.InfoAsync("Update ResultMarkets: " + body);
            return Results.Ok(new { IsSuccess = true, Message = string.Empty });
        });

        app.MapPost("api/cash-out/VoidBet/UndoVoidBet", async (HttpContext context) =>
        {
            using var reader = new StreamReader(context.Request.Body);
            var body = await reader.ReadToEndAsync();
            await Fanex.Logging.LogService.InfoAsync("UndoVoidBet: " + body);
            return Results.Ok(body);
        });
        app.MapPost("api/cash-out/VoidBet/VoidBet", async (HttpContext context) =>
        {
            using var reader = new StreamReader(context.Request.Body);
            var body = await reader.ReadToEndAsync();
            await Fanex.Logging.LogService.InfoAsync("VoidBet: " + body);
            return Results.Ok(body);
        });

        app.MapPost("api/cash-out/UndoVoidBet", async (HttpContext context) =>
        {
            using var reader = new StreamReader(context.Request.Body);
            var body = await reader.ReadToEndAsync();
            await Fanex.Logging.LogService.InfoAsync("UndoVoidBet: " + body);
            return Results.Ok(body);
        });

        app.MapPost("api/cash-out/CashOutResultUpdate", async (HttpContext context) =>
        {
            using var reader = new StreamReader(context.Request.Body);
            var body = await reader.ReadToEndAsync();
            await Fanex.Logging.LogService.InfoAsync("CashOutResultUpdate: " + body);
            return Results.Ok(body);
        });

        app.MapPost("api/cash-out/CashOutResultRefund", async (HttpContext context) =>
        {
            using var reader = new StreamReader(context.Request.Body);
            var body = await reader.ReadToEndAsync();
            await Fanex.Logging.LogService.InfoAsync("CashOutResultRefund: " + body);
            return Results.Ok(body);
        });

    }
}