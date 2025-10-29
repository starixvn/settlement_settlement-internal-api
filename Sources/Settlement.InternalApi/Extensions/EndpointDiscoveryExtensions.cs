using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using MinimalApi.Endpoint;

namespace Settlement.InternalApi.Extensions;

public static class EndpointDiscoveryExtensions
{
    public static IServiceCollection AddEndpointsFromAssemblies(this IServiceCollection services, params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            var endpointTypes = assembly.GetTypes()
                .Where(type => !type.IsAbstract && 
                              !type.IsInterface && 
                              typeof(IEndpoint).IsAssignableFrom(type))
                .ToList();

            foreach (var endpointType in endpointTypes)
            {
                services.AddTransient(endpointType);
            }
        }

        return services;
    }

    public static IApplicationBuilder MapEndpointsFromAssemblies(this IApplicationBuilder app, params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            var endpointTypes = assembly.GetTypes()
                .Where(type => !type.IsAbstract && 
                              !type.IsInterface && 
                              typeof(IEndpoint).IsAssignableFrom(type))
                .ToList();

            foreach (var endpointType in endpointTypes)
            {
                var endpoint = app.ApplicationServices.GetRequiredService(endpointType);
                if (endpoint is IEndpoint<IResult> endpointWithResult)
                {
                    endpointWithResult.AddRoute((IEndpointRouteBuilder)app);
                }
            }
        }

        return app;
    }
}
