using Fanex.Data.Repository;
using Settlement.Gaming.Criteria.Happy5;
using Settlement.Gaming.Models;
using Settlement.Gaming.Models.Happy5;
using Settlement.Gaming.Requests;

namespace Settlement.Gaming.Services.Happy5;

public interface BallService
{
    Task<List<BallModel>> GetBalls(string matchIds);

    Task<List<BallModel>> GetBallsByMatchId(int matchId);

    List<BallModel> CorrectNumberBall(int matchId, List<BallModel> balls);

    Task UpdateBalls(HappyFiveRequest request);
}

public class BallServiceImp(IDynamicRepository repository, XmlConvertService xmlConvertService)
    : BallService
{
    public async Task<List<BallModel>> GetBalls(string matchIds)
    {
        var criteria = new GetBallCriteria
        {
            MatchIds = matchIds
        };

        var ball = await repository.FetchAsync<BallModel>(criteria);

        return ball.ToList();
    }

    public async Task<List<BallModel>> GetBallsByMatchId(int matchId)
    {
        var balls = await GetBalls(matchId.ToString());

        return CorrectNumberBall(matchId, balls);
    }

    public List<BallModel> CorrectNumberBall(int matchId, List<BallModel> balls)
    {
        var fiveBall = new List<byte> { 1, 2, 3, 4, 5 };

        return fiveBall.Select(item => new BallModel(matchId, item, GetBallNumber(item))).ToList();

        byte GetBallNumber(byte item)
        {
            var currentBall = balls.Find(b => b.Seq == item);
            var number = currentBall?.NumberBall ?? 0;
            return number;
        }
    }

    public async Task UpdateBalls(HappyFiveRequest request)
    {
        var updateMatchCriteria = new UpdateResultCriteria
        {
            MatchId = request.MatchId,
            Resources = ResourcesBalls(request.Balls)
        };

        await repository.ExecuteAsync(updateMatchCriteria);
    }

    public string ResourcesBalls(List<BallModel> balls)
    {
        var resourceItems = balls.Select(ball => new ResourcesItem { Seq = ball.Seq, NumberBall = ball.NumberBall }).ToList();

        var resources = new Resources
        {
            ResourceItems = resourceItems
        };

        return xmlConvertService.Serialize(resources);
    }
}