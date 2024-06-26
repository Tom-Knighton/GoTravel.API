using GoTravel.API.Domain.Exceptions;
using GoTravel.API.Domain.Models.Database;
using GoTravel.API.Domain.Models.DTOs;
using GoTravel.API.Domain.Models.Lib;
using GoTravel.API.Domain.Services;
using GoTravel.API.Domain.Services.Auth;
using GoTravel.API.Domain.Services.Mappers;
using GoTravel.API.Domain.Services.Repositories;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace GoTravel.API.Services.Services;

public class ScoreboardService: IScoreboardService
{
    private readonly IScoreboardRepository _repo;
    private readonly IUserService _userService;
    private readonly IMapper<GTScoreboard, ScoreboardDto> _map;
    private readonly IMapper<GTUserDetails, UserDto> _userMap;
    private readonly ILogger<ScoreboardService> _log;
    private readonly TimeProvider _time;
    private readonly IPublishEndpoint _publisher;
    private const string MostTravelName = "Most Travel";

    public ScoreboardService(IScoreboardRepository repo, IUserService userService, IMapper<GTScoreboard, ScoreboardDto> map, IMapper<GTUserDetails, UserDto> userMap, ILogger<ScoreboardService> log, TimeProvider time, IPublishEndpoint publisher)
    {
        _repo = repo;
        _log = log;
        _map = map;
        _userMap = userMap;
        _time = time;
        _publisher = publisher;
        _userService = userService;
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
        var scoreboard = await _repo.GetScoreboard(scoreboardId, ct);

        if (scoreboard is null)
        {
            throw new ScoreboardNotFoundException(scoreboardId);
        }

        var dto = _map.Map(scoreboard);

        return dto;
    }

    public async Task<ICollection<ScoreboardUserDto>> GetScoreboardUsers(string scoreboardId, int fromPosition, int results, CancellationToken ct = default)
    {
        var users = await _repo.GetUsersForScoreboard(scoreboardId, fromPosition, results, ct);

        var dtos = users.Select(u => new ScoreboardUserDto
            {
                Rank = u.rank,
                Points = u.user.Points,
                User = _userMap.Map(u.user.User)
            })
            .OrderBy(u => u.Rank);

        return dtos.ToList();
    }

    public async Task ResetAnyScoreboards(CancellationToken ct = default)
    {
        var scoreboardsToReset = await _repo.GetScoreboardsToReset(ct);

        foreach (var sb in scoreboardsToReset)
        {
            var originalDifference = sb.EndsAt - sb.ActiveFrom;
            sb.ActiveFrom = _time.GetUtcNow().UtcDateTime;
            sb.EndsAt = originalDifference == null ? null : _time.GetUtcNow().UtcDateTime.Add(originalDifference.Value);

            var winner = sb.Users.MaxBy(u => u.Points);
            if (winner is not null)
            {
                var random = new Random();
                var rewards = Enum.GetValues<ScoreboardWinRewardType>();
                var win = new GTScoreboardWin
                {
                    UUID = Guid.NewGuid().ToString("N"),
                    UserId = winner.UserId,
                    ScoreboardId = winner.ScoreboardUUID,
                    HasSeen = false,
                    ScoreboardPosition = 1,
                    WonAt = _time.GetUtcNow().UtcDateTime,
                    RewardType = rewards[random.Next(0, rewards.Length)]
                };
                await _repo.SaveWin(win, ct);
            }
        }

        await _repo.SaveScoreboards(scoreboardsToReset, ct);
    }

    public async Task AddToTravelBoard(string userId, int points, CancellationToken ct = default)
    {
        var scoreboard = await _repo.GetSingleByName(MostTravelName, ct);
        if (scoreboard is null)
        {
            _log.LogCritical("Most Travel scoreboard could not be found!");
            throw new NullReferenceException(nameof(scoreboard));
        }

        var user = await _repo.GetUserInScoreboard(scoreboard.UUID, userId, ct);
        if (user is null)
        {
            user = new GTScoreboardUser
            {
                UserId = userId,
                Points = points,
                JoinedAt = _time.GetUtcNow().UtcDateTime,
                ScoreboardUUID = scoreboard.UUID
            };
        }
        else
        {
            user.Points += points;
        }

        await _repo.SaveUser(user, ct);
    }

    public async Task SeenWin(string winId, string userId, CancellationToken ct = default)
    {
        var win = await _repo.GetWin(winId, ct);

        if (win is null)
        {
            throw new WinNotFoundException(winId);
        }

        if (win.UserId != userId)
        {
            throw new WrongUserException(userId);
        }

        win.HasSeen = true;

        switch (win.RewardType)
        {
            case ScoreboardWinRewardType.Title_SuperTraveller:
                var stTitle = new GTUserSubtitle
                    { UserId = userId, ExpiresAt = _time.GetUtcNow().UtcDateTime.AddDays(14), IsActive = true, ValidFrom = _time.GetUtcNow().UtcDateTime, Title = "Super Traveller!" };
                await _userService.AddSubtitle(userId, stTitle, ct);
                break;
            case ScoreboardWinRewardType.Title_PublicTransportExpert:
                var pteTitle = new GTUserSubtitle
                    { UserId = userId, ExpiresAt = _time.GetUtcNow().UtcDateTime.AddDays(14), IsActive = true, ValidFrom = _time.GetUtcNow().UtcDateTime, Title = "Public Transport Expert!" };
                await _userService.AddSubtitle(userId, pteTitle, ct);
                break;
            case ScoreboardWinRewardType.StartingPoints_10:
                await _publisher.Publish(new AddPointsMessage { UserId = userId, Message = "Won Most Travel Competition", Points = 10, ReasonType = AddPointsReasonType.Travel }, ct);
                break;
            case ScoreboardWinRewardType.StartingPoints_30:
                await _publisher.Publish(new AddPointsMessage { UserId = userId, Message = "Won Most Travel Competition", Points = 30, ReasonType = AddPointsReasonType.Travel }, ct);
                break;
            case ScoreboardWinRewardType.None:
            case ScoreboardWinRewardType.PointMultiplier_1_5:
            case ScoreboardWinRewardType.PointMultiplier_2:
            default:
                break;
        }
        
        await _repo.SaveWin(win, ct);
    }

    public async Task<ICollection<ScoreboardWinDto>> GetUnseenWinsForUser(string userId, CancellationToken ct = default)
    {
        var wins = await _repo.GetUnseenWinsForUser(userId, ct);

        var dtos = wins.Select(w => new ScoreboardWinDto
        {
            WinId = w.UUID,
            ScoreboardName = w.Scoreboard.ScoreboardName,
            WonAt = w.WonAt,
            Position = w.ScoreboardPosition,
            RewardType = w.RewardType
        });

        return dtos.ToList();
    }

    public async Task<ICollection<ScoreboardWinDto>> GetSeenWinsForUser(string userId, CancellationToken ct = default)
    {
        var wins = await _repo.GetAppliedWins(userId, ct);

        var dtos = wins.Select(w => new ScoreboardWinDto
        {
            WinId = w.UUID,
            ScoreboardName = w.Scoreboard.ScoreboardName,
            WonAt = w.WonAt,
            Position = w.ScoreboardPosition,
            RewardType = w.RewardType
        });

        return dtos.ToList();
    }
}