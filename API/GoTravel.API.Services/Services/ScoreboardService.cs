using GoTravel.API.Domain.Models.Database;
using GoTravel.API.Domain.Models.DTOs;
using GoTravel.API.Domain.Services;
using GoTravel.API.Domain.Services.Mappers;
using GoTravel.API.Domain.Services.Repositories;
using Microsoft.Extensions.Logging;

namespace GoTravel.API.Services.Services;

public class ScoreboardService: IScoreboardService
{
    private readonly IScoreboardRepository _repo;
    private readonly IMapper<GTScoreboard, ScoreboardDto> _map;
    private readonly IMapper<GTUserDetails, UserDto> _userMap;
    private readonly ILogger<ScoreboardService> _log;

    public ScoreboardService(IScoreboardRepository repo, IMapper<GTScoreboard, ScoreboardDto> map, IMapper<GTUserDetails, UserDto> userMap, ILogger<ScoreboardService> log)
    {
        _repo = repo;
        _log = log;
        _map = map;
        _userMap = userMap;
    }

    public async Task<ICollection<ScoreboardDto>> GetScoreboardsForUser(string userId, CancellationToken ct = default)
    {
        var scoreboards = await _repo.GetScoreboardsForUser(userId, ct);

        var mapped = new List<ScoreboardDto>();
        foreach (var sb in scoreboards)
        {
            var dto = _map.Map(sb);
            if (sb.Users.All(u => u.UserId != userId))
            {
                var userRank = await _repo.GetUserPositionInScoreboard(sb.UUID, userId, ct);
                if (userRank is not null)
                {
                    sb.Users.Add(userRank.Item2);
                    dto.ScoreboardUsers.Add(new ScoreboardUserDto
                    {
                        Rank = userRank.Item1,
                        Points = userRank.Item2.Points,
                        User = _userMap.Map(userRank.Item2.User)
                    });
                }
            }
            mapped.Add(dto);
        }

        return mapped;
    }

    public async Task<ScoreboardDto?> GetScoreboard(string scoreboardId, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task<ICollection<ScoreboardUserDto>> GetScoreboardUsers(string scoreboardId, int fromPosition, int results, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}