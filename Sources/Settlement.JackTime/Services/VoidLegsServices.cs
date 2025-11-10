using Google.Protobuf;
using Microsoft.Extensions.Options;
using Settlement.Internal.Models;
using Settlement.JackTime.Constants;
using Settlement.JackTime.Models;
using Settlement.JackTime.Requests;

namespace Settlement.JackTime.Services;

public interface VoidLegsServices
{
    Task<Results<string>> VoidBetAsync(VoidBetInfoRequest voidBetInfoRequest);

    Task<Results<string>> VoidLegsAsync(VoidLegRequest voidLegRequest);

    Task<Results<string>> UnVoidLegsAsync(VoidLegRequest voidLegRequest);
}

public class VoidLegsServicesImp(
    IOptions<AppSettingsKey> appSettingsKey,
    InvokeExternalAPIServices invokeExternalApiServices,
    GenerateServices generateServices,
    CreateRequestServices commandRequestServices) : VoidLegsServices
{
    public async Task<Results<string>> VoidBetAsync(VoidBetInfoRequest voidBetInfoRequest)
    {
        return await ProcessVoidBetInfosAsync(voidBetInfoRequest.VoidBetInfos, VoidBet);
    }

    public async Task<Results<string>> VoidLegsAsync(VoidLegRequest voidLegRequest)
    {
        return await ProcessVoidBetInfosAsync(voidLegRequest.VoidBetInfos, VoidLegs);
    }

    public async Task<Results<string>> UnVoidLegsAsync(VoidLegRequest voidLegRequest)
    {
        return await ProcessVoidBetInfosAsync(voidLegRequest.VoidBetInfos, UnVoidLegs);
    }

    private async Task<Results<string>> ProcessVoidBetInfosAsync(
        IEnumerable<VoidBetInfo> voidBetInfos,
        Func<VoidBetInfo, Task<Results<string>>> processMethod)
    {
        var tasks = voidBetInfos.Select(processMethod).ToList();
        var results = await Task.WhenAll(tasks.ToArray());
        var resultFailed = results.Where(r => !r.IsSuccessful).ToList();

        return new Results<string>
        {
            ErrMsg = resultFailed.Count > 0
                ? string.Join(" | ", resultFailed.Select(r => r.ErrMsg).ToArray())
                : Result.Success
        };
    }

    private async Task<Results<string>> VoidBet(VoidBetInfo voidBetInfo)
    {
        return await ProcessSingleVoidBetAsync(voidBetInfo, commandRequestServices.CreateVoidBetCommandRequest);
    }

    private async Task<Results<string>> VoidLegs(VoidBetInfo voidBetInfo)
    {
        return await ProcessSingleVoidBetAsync(voidBetInfo, commandRequestServices.CreateVoidCommandRequest);
    }

    private async Task<Results<string>> UnVoidLegs(VoidBetInfo voidBetInfo)
    {
        return await ProcessSingleVoidBetAsync(voidBetInfo, commandRequestServices.CreateUnVoidCommandRequest);
    }

    private async Task<Results<string>> ProcessSingleVoidBetAsync(
        VoidBetInfo voidBetInfo,
        Func<VoidBetInfo, CommandRequestMessage> createCommandRequest)
    {
        var commandRequest = createCommandRequest(voidBetInfo);
        var requestString = new JsonFormatter(JsonFormatter.Settings.Default).Format(commandRequest);

        var url = $"{appSettingsKey.Value.ThirdPartyUri.JackTimeCommand}";

        var response = await invokeExternalApiServices.PostAsync(requestString, url);
        LogService.Info($"JackTime VoidLegsServices Response: {requestString} - {response}");

        return generateServices.CommandReplyMessageAsync(response);
    }
}