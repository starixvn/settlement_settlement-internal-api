using Microsoft.Extensions.Options;
using Settlement.Gaming.Models;
using Settlement.Gaming.Requests;
using Settlement.Internal.Models;
using System.Diagnostics;
using BusinessModels = Settlement.Business.Core.Models;

namespace Settlement.Gaming.Services.Happy5;

public interface SettleService
{
    Task<Result> Settle(HappyFiveRequest request);
}

public class SettleServiceImp(
    IOptions<AppSettingsKey> appSettingsKey,
    ProcessService processService,
    BallService ballService,
    MatchService matchService,
    LogService logService,
    Business.Data.Services.MatchService businessMatchService)
    : SettleService
{
    public async Task<Result> Settle(HappyFiveRequest request)
    {
        request.OldResult = await GetCurrentBalls(request.MatchId);

        await ballService.UpdateBalls(request);
        await matchService.UpdateMatchInfo(request);
        WriteLog(request.MatchId, request.UserId);

        var (resources, balls) = await businessMatchService.GetResourceHappy5(request.MatchId, request.Action);

        var watch = Stopwatch.StartNew();
        var result = await SettleBySite(request, resources.ToList());
        watch.Stop();

        await WriteLogSettled(request, result, balls, watch.ElapsedMilliseconds);

        return result;
    }

    private async Task<Result> SettleBySite(HappyFiveRequest request, List<BusinessModels.Resources> resources)
    {
        var tasks = appSettingsKey.Value
            .SiteDbServer
            .Select(site => processService.Settle(request, site, resources)).ToList();

        var results = await Task.WhenAll(tasks.ToArray());
        var resultFailed = results.Where(r => r.ErrCode > 0).ToList();

        return new Result
        {
            ErrCode = resultFailed.Count,
            ErrMsg = string.Join(" | ", resultFailed.Select(r => r.ErrMessage).ToArray())
        };
    }

    private async Task WriteLogSettled(HappyFiveRequest request, Result result, IEnumerable<BusinessModels.Ball> balls, long duration)
    {
        var newBalls = string.Join(", ", balls.Where(ball => ball.Number > 0).Select(ball => $"B{ball.Seq}({ball.Number})").ToArray());
        var message = result.ErrCode > 0 ? $"<br/>{result.ErrMsg}" : string.Empty;

        var results = string.IsNullOrWhiteSpace(newBalls) ? string.Empty : $"Results: {newBalls}<br/>";

        var remark = $"IsAuto: 0<br/>{request.OldResult}{results}Duration: {duration} ms <br/>{message}";

        await logService.WriteLog(request.MatchId, remark, request.ActionId, request.UserId, request.Ip);
    }

    private async Task<string> GetCurrentBalls(int matchId)
    {
        var balls = await ballService.GetBallsByMatchId(matchId);
        var currentBalls = string.Join(", ", balls.Where(ball => ball.NumberBall > 0).Select(ball => $"B{ball.Seq}({ball.NumberBall})").ToArray());

        return string.IsNullOrWhiteSpace(currentBalls) ? string.Empty : $"Old Results: {currentBalls}<br/>";
    }

    private void WriteLog(int matchId, int userId)
    {
        var messageLog = $"Settlement for matchId: {matchId}";
        logService.WriteLog(matchId, messageLog, userId);
    }
}