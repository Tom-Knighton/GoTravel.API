using GoTravel.API.Domain.Models.DTOs;

namespace GoTravel.API.Domain.Services;

public interface IScoreboardService
{
    public Task<ICollection<ScoreboardDto>> GetScoreboardsForUser(string userId, CancellationToken ct = default);

    public Task<ScoreboardDto?> GetScoreboard(string scoreboardId, CancellationToken ct = default);

    public Task<ICollection<ScoreboardUserDto>> GetScoreboardUsers(string scoreboardId, int fromPosition, int results, CancellationToken ct = default);

    public Task ResetAnyScoreboards(CancellationToken ct = default);

    public Task AddToTravelBoard(string userId, int points, CancellationToken ct = default);
    public Task SeenWin(string winId, string userId, DateTime at, CancellationToken ct = default);
    public Task<ICollection<ScoreboardWinDto>> GetUnseenWinsForUser(string userId, CancellationToken ct = default);
}