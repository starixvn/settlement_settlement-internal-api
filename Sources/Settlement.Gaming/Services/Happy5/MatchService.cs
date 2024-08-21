using Fanex.Data.Repository;
using Fanex.Settlement.ScaleOut.Request;
using Fanex.Settlement.ScaleOut.Services;
using Settlement.Gaming.Criteria.Happy5;
using Settlement.Gaming.Models;
using Settlement.Gaming.Models.Happy5;
using Settlement.Gaming.Requests;

namespace Settlement.Gaming.Services.Happy5;

public interface MatchService
{
    Task UpdateMatchInfo(HappyFiveRequest request);

    Task<List<MatchDetail>> Matches(GetMatchesCriteria criteria);
}

public class MatchServiceImp(
    IDynamicRepository repository,
    ScaleOutSettleThirdPartyService scaleOutSettleThirdPartyService,
    BallService ballService)
    : MatchService
{
    public async Task UpdateMatchInfo(HappyFiveRequest request)
    {
        var updateRequest = new UpdateMatchInfo3RdRequest
        {
            Command = "completed",
            Action = request.Action,
            MatchId = request.MatchId,
            Creator = request.UserId
        };

        await scaleOutSettleThirdPartyService.UpdateMatchInfo3rd(updateRequest);
    }

    public async Task<List<MatchDetail>> Matches(GetMatchesCriteria criteria)
    {
        var matches = (await repository.FetchAsync<MatchDetail>(criteria)).ToList();
        var balls = await GetBallsByMatchIds(matches);

        matches.ForEach(m => m.Balls = balls.TryGetValue(m.MatchId, out var ball) ? ballService.CorrectNumberBall(m.MatchId, ball) : []);

        return matches;
    }

    private async Task<Dictionary<int, List<BallModel>>> GetBallsByMatchIds(List<MatchDetail> matches)
    {
        if (matches.Any())
        {
            var matchIdsAsString = string.Join(",", matches.Select(x => x.MatchId));
            var balls = await ballService.GetBalls(matchIdsAsString);
            return balls.GroupBy(b => b.MatchId)
                .ToDictionary(m => m.Key, m => m.ToList());
        }

        return new Dictionary<int, List<BallModel>>();
    }
}