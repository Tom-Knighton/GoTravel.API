using EFCore.BulkExtensions;
using GoTravel.API.Domain.Data;
using GoTravel.API.Domain.Models.Database;
using GoTravel.API.Domain.Services.Repositories;
using MassTransit.Internals;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Zomp.EFCore.WindowFunctions;

namespace GoTravel.API.Services.Services.Repositories;

public class ScoreboardRepository: IScoreboardRepository
{
    private readonly GoTravelContext _context;
    private readonly ILogger<ScoreboardRepository> _log;

    public ScoreboardRepository(GoTravelContext context, ILogger<ScoreboardRepository> log)
    {
        _context = context;
        _log = log;
    }

    public async Task<ICollection<GTScoreboard>> GetScoreboardsForUser(string userId, CancellationToken ct = default)
    {
        var results = await _context.Scoreboards
            .Include(s => s.Users.OrderByDescending(u => u.Points).Take(1))
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
}
