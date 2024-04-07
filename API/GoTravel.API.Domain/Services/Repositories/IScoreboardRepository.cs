using System.Collections;
using GoTravel.API.Domain.Models.Database;

namespace GoTravel.API.Domain.Services.Repositories;

public interface IScoreboardRepository
{
    Task<ICollection<GTScoreboard>> GetScoreboardsForUser(string userId, CancellationToken ct = default);
    Task<Tuple<int, GTScoreboardUser>?> GetUserPositionInScoreboard(string scoreboardId, string userId, CancellationToken ct = default);
    Task<ICollection<(int rank, GTScoreboardUser user)>> GetUsersForScoreboard(string scoreboardId, int startFrom, int results, CancellationToken ct = default);

    Task<GTScoreboard?> GetScoreboard(string scoreboardId, CancellationToken ct = default);

    Task<ICollection<GTScoreboard>> GetScoreboardsToReset(CancellationToken ct = default);
    Task SaveScoreboards(ICollection<GTScoreboard> scoreboards, CancellationToken ct = default);

    Task<GTScoreboard?> GetSingleByName(string name, CancellationToken ct = default);
    Task<GTScoreboardUser?> GetUserInScoreboard(string scoreboardId, string userId, CancellationToken ct = default);
    Task SaveUser(GTScoreboardUser user, CancellationToken ct = default);

    Task<GTScoreboardWin?> GetWin(string winId, CancellationToken ct = default);
    Task SaveWin(GTScoreboardWin win, CancellationToken ct = default);
    Task<ICollection<GTScoreboardWin>> GetUnseenWinsForUser(string userId, CancellationToken ct = default);
    Task<ICollection<GTScoreboardWin>> GetAppliedWins(string userId, CancellationToken ct = default);
}