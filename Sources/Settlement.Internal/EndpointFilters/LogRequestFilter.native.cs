using Fanex.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Settlement.Internal.Models;
using System.Text;

namespace Settlement.Internal.EndpointFilters;

public class LogRequestFilter(IOptions<AppSettingsKey> appSettingsKey) : IEndpointFilter
{
    public async ValueTask<object> InvokeAsync(EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        if (appSettingsKey.Value.EnableLogRequest)
        {
            var model = context.Arguments[0];
            var serialString = JsonConvert.SerializeObject(model);
            var url = context.HttpContext.Request.GetDisplayUrl();
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"InternalApi - Url: {url}");
            stringBuilder.AppendLine($"Params: {serialString}");

            await new Logger().InfoAsync(stringBuilder.ToString());
        }

        return await next(context);
    }
}