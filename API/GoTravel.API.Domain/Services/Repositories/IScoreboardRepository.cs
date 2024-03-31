using GoTravel.API.Domain.Models.Database;

namespace GoTravel.API.Domain.Services.Repositories;

public interface IScoreboardRepository
{
    Task<ICollection<GTScoreboard>> GetScoreboardsForUser(string userId, CancellationToken ct = default);
    Task<Tuple<int, GTScoreboardUser>?> GetUserPositionInScoreboard(string scoreboardId, string userId, CancellationToken ct = default);
    Task<ICollection<(int rank, GTScoreboardUser user)>> GetUsersForScoreboard(string scoreboardId, int startFrom, int results, CancellationToken ct = default);

    Task<GTScoreboard?> GetScoreboard(string scoreboardId, CancellationToken ct = default);
    

}