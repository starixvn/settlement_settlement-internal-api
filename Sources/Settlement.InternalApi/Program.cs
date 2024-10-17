using Fanex.Data.SqlServer;
using Fanex.Logging.Sentry.RavenClient;
using MinimalApi.Endpoint.Extensions;
using ONEbook.UM.DependencyInjection;
using Settlement.Internal.Models;
using Settlement.InternalApi;
using Settlement.InternalApi.Config;
using Settlement.InternalApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpoints();

builder.Services.AddHealthChecks();

builder.Services.AddFanexSql(builder.Configuration);

builder.Services.AddOneBookUm(builder.Configuration);

builder.Services.AddFanexLogSentry(builder.Configuration);

builder.Services.AddConfigRedisCache(builder.Configuration);

builder.Services.AddInitialServices(builder.Configuration);

builder.Services.Configure<AppSettingsKey>(builder.Configuration);

builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHealthChecks("/health");

app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseRouting();

app.MapEndpoints();

await app.RunAsync();