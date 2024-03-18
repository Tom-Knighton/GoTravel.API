using GoTravel.API.Domain.Data;
using GoTravel.API.Domain.Models.Database;
using GoTravel.API.Domain.Services.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Zomp.EFCore.WindowFunctions;

namespace GoTravel.API.Services.Services.Repositories;

public class ScoreboardRepository: IScoreboardRepository
{
    private readonly GoTravelContext _context;
    private readonly ILogger<ScoreboardRepository> _log;

    private const int InitialUsersToTake = 10;

    public ScoreboardRepository(GoTravelContext context, ILogger<ScoreboardRepository> log)
    {
        _context = context;
        _log = log;
    }

    public async Task<ICollection<GTScoreboard>> GetScoreboardsForUser(string userId, CancellationToken ct = default)
    {
        var results = await _context.Scoreboards
            .Include(s => s.Users.OrderByDescending(u => u.Points).Take(InitialUsersToTake))
            .ThenInclude(u => u.User)
            .Where(s => s.Users.Any(u => u.UserId == userId))
            .ToListAsync(ct);
        
        return results;
    }

    public async Task<Tuple<int, GTScoreboardUser>?> GetUserPositionInScoreboard(string scoreboardId, string userId, CancellationToken ct = default)
    {
        var rankQuery = await _context.ScoreboardUsers
            .Where(u => u.ScoreboardUUID == scoreboardId)
            .Select(u => new
            {
                u.UserId,
                Rank = (int)EF.Functions.Rank(EF.Functions.Over().OrderByDescending(u.Points))
            })
            .ToListAsync(ct);

        var rank = rankQuery.FirstOrDefault(u => u.UserId == userId);
        if (rank is null)
        {
            return null;
        }
        
        var user = await _context.ScoreboardUsers
            .Include(u => u.User)
            .FirstOrDefaultAsync(u => u.ScoreboardUUID == scoreboardId && u.UserId == userId, ct);        
        
        return new Tuple<int, GTScoreboardUser>(rank.Rank, user);
    }

    public async Task<ICollection<(int rank, GTScoreboardUser user)>> GetUsersForScoreboard(string scoreboardId, int startFrom, int results, CancellationToken ct = default)
    {
        var start = Math.Max(0, startFrom - 1);
        var take = Math.Max(0, results);
        var users = await _context.ScoreboardUsers
            .Where(u => u.ScoreboardUUID == scoreboardId)
            .Include(u => u.User)
            .Select(u => new
            {
                Rank = (int)EF.Functions.Rank(EF.Functions.Over().OrderByDescending(u.Points)),
                User = u
            })
            .OrderBy(u => u.Rank)
            .ToListAsync(ct);

        return users.Skip(start).Take(take).Select(r => (r.Rank, r.User)).ToList();
    }

    public async Task<GTScoreboard?> GetScoreboard(string scoreboardId, CancellationToken ct = default)
    {
        return await _context.Scoreboards
            .Include(s => s.Users.OrderByDescending(u => u.Points).Take(InitialUsersToTake))
            .ThenInclude(u => u.User)
            .FirstOrDefaultAsync(s => s.UUID == scoreboardId, ct);
    }
}
