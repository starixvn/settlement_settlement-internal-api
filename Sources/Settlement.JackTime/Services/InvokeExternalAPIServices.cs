using System.Text;
using Polly;

namespace Settlement.JackTime.Services;

public interface InvokeExternalAPIServices
{
    Task<string> PostAsync(string requestString, string apiUrl);
}

public class InvokeExternalAPIServicesImp(IHttpClientFactory httpClientFactory) : InvokeExternalAPIServices
{
    public async Task<string> PostAsync(string requestString, string apiUrl)
    {
        var policy = Policy
           .Handle<HttpRequestException>()
           .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        var httpClient = httpClientFactory.CreateClient();
        var requestContent = new StringContent(requestString, Encoding.UTF8, "application/json");

        return await policy.ExecuteAsync(async () =>
        {
            var response = await httpClient.PostAsync(new Uri(apiUrl), requestContent);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        });
    }
}