using GoTravel.API.Domain.Models.DTOs;

namespace GoTravel.API.Domain.Services;

public interface IScoreboardService
{
    public Task<ICollection<ScoreboardDto>> GetScoreboardsForUser(string userId, CancellationToken ct = default);

    public Task<ScoreboardDto?> GetScoreboard(string scoreboardId, CancellationToken ct = default);

    public Task<ICollection<ScoreboardUserDto>> GetScoreboardUsers(string scoreboardId, int fromPosition, int results, CancellationToken ct = default);

    public Task ResetAnyScoreboards(CancellationToken ct = default);
}