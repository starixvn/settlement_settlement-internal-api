using AutoMapper;
using Fanex.Data.Repository;
using Settlement.Gaming.Criteria.Happy5;
using Settlement.Gaming.Models;
using Settlement.Gaming.Models.Happy5;

namespace Settlement.Gaming.Services.Happy5;

public interface MatchService
{
    Task<List<MatchDetail>> Matches(GetMatchesCriteria criteria);
}

public class MatchServiceImp(
    IDynamicRepository repository,
    BallService ballService,
    IMapper mapper)
    : MatchService
{
    public async Task<List<MatchDetail>> Matches(GetMatchesCriteria criteria)
    {
        var matches = (await repository.FetchAsync<MatchDetail>(criteria)).ToList();
        var balls = await GetBallsByMatchIds(matches);

        matches.ForEach(m => m.Balls = balls.TryGetValue(m.MatchId, out var ball) ? ballService.CorrectNumberBall(m.MatchId, ball) :
        [
        ]);

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