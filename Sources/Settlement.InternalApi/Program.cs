using Fanex.Data.SqlServer;
using Fanex.Logging.Sentry.RavenClient;
using MinimalApi.Endpoint.Extensions;
using ONEbook.UM.DependencyInjection;
using Settlement.Internal.Models;
using Settlement.InternalApi;
using Settlement.InternalApi.Config;
using Settlement.InternalApi.Middleware;
using Settlement.JackTime;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddEndpoints();

services.AddHealthChecks();

services.AddHttpClient();

services.AddFanexSql(builder.Configuration);

services.AddOneBookUmForService(builder.Configuration);

services.AddFanexLogSentry(builder.Configuration);

services.AddInitialServices(builder.Configuration);

services.JackTimeConfigServices();

services.Configure<AppSettingsKey>(builder.Configuration);

services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

var app = builder.Build();
app.Use(async (context, next) =>
{
    context.RequestAborted = new CancellationTokenSource(TimeSpan.FromMinutes(10)).Token;
    await next();
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHealthChecks("/healthz");

app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseRouting();

app.MapEndpoints();

await app.RunAsync();