using MinimalApi.Endpoint.Extensions;
using Settlement.Internal.Models;
using Settlement.InternalApi;
using Settlement.InternalApi.Config;
using Settlement.InternalApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpoints();

builder.Services.AddHealthChecks();

builder.Services.Configure<AppSettingsKey>(builder.Configuration);

builder.Services.AddInitialServices(builder.Configuration);

builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHealthChecks("/health");

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionMiddleware>();

app.UseRouting();

app.UseCors("AllowOrigin");

app.MapEndpoints();

app.Run();