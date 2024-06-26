using EFCore.BulkExtensions;
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
    private readonly TimeProvider _time;
    
    private const int InitialUsersToTake = 10;

    public ScoreboardRepository(GoTravelContext context, ILogger<ScoreboardRepository> log, TimeProvider time)
    {
        _context = context;
        _log = log;
        _time = time;
    }

    public async Task<ICollection<GTScoreboard>> GetScoreboardsForUser(string userId, CancellationToken ct = default)
    {
        var results = await _context.Scoreboards
            .Include(s => s.Users.OrderByDescending(u => u.Points).Take(InitialUsersToTake))
            .ThenInclude(u => u.User)
            .Where(s => s.JoinType == GTScoreboadJoinType.AllEnrolled || s.Users.Any(u => u.UserId == userId))
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

    public async Task<ICollection<GTScoreboard>> GetScoreboardsToReset(CancellationToken ct = default)
    {
        return await _context.Scoreboards
            .Include(s => s.Users)
            .Where(s => s.EndsAt < _time.GetUtcNow().UtcDateTime && s.DoesReset)
            .ToListAsync(ct);
    }

    public async Task SaveScoreboards(ICollection<GTScoreboard> scoreboards, CancellationToken ct = default)
    {
        await _context.BulkUpdateAsync(scoreboards.ToList(), b =>
        {
            b.IncludeGraph = true;
        }, cancellationToken: ct);

        await _context.BulkSaveChangesAsync(cancellationToken: ct);
    }

    public async Task<GTScoreboard?> GetSingleByName(string name, CancellationToken ct = default)
    {
        return await _context.Scoreboards
            .FirstOrDefaultAsync(s => s.ScoreboardName == name, ct);
    }

    public async Task<GTScoreboardUser?> GetUserInScoreboard(string scoreboardId, string userId, CancellationToken ct = default)
    {
        return await _context.ScoreboardUsers
            .FirstOrDefaultAsync(s => s.ScoreboardUUID == scoreboardId && s.UserId == userId, ct);
    }

    public async Task SaveUser(GTScoreboardUser user, CancellationToken ct = default)
    {
        if (await _context.ScoreboardUsers.AnyAsync(u => u.UserId == user.UserId && u.ScoreboardUUID == user.ScoreboardUUID, ct))
        {
            _context.ScoreboardUsers.Update(user);
        }
        else
        {
            _context.ScoreboardUsers.Add(user);
        }

        await _context.SaveChangesAsync(ct);
    }

    public async Task<GTScoreboardWin?> GetWin(string winId, CancellationToken ct = default)
    {
        return await _context.ScoreboardWins
            .FirstOrDefaultAsync(w => w.UUID == winId, ct);
    }

    public async Task SaveWin(GTScoreboardWin win, CancellationToken ct = default)
    {
        if (await _context.ScoreboardWins.AnyAsync(w => w.UUID == win.UUID, ct))
        {
            _context.ScoreboardWins.Update(win);
        }
        else
        {
            _context.ScoreboardWins.Add(win);
        }

        await _context.SaveChangesAsync(ct);
    }

    public async Task<ICollection<GTScoreboardWin>> GetUnseenWinsForUser(string userId, CancellationToken ct = default)
    {
        return await _context.ScoreboardWins
            .Where(w => w.UserId == userId && !w.HasSeen)
            .Include(s => s.Scoreboard)
            .ToListAsync(ct);
    }

    public async Task<ICollection<GTScoreboardWin>> GetAppliedWins(string userId, CancellationToken ct = default)
    {
        var twoWeeksAgo = _time.GetUtcNow().UtcDateTime.AddDays(-14);
        return await _context.ScoreboardWins
            .Include(s => s.Scoreboard)
            .Where(w => w.UserId == userId && w.WonAt > twoWeeksAgo)
            .ToListAsync(ct);
    }
}
