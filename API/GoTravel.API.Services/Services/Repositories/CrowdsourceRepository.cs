using GoTravel.API.Domain.Data;
using GoTravel.API.Domain.Models.Database;
using GoTravel.API.Domain.Services.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GoTravel.API.Services.Services.Repositories;

public class CrowdsourceRepository: ICrowdsourceRepository
{
    private readonly GoTravelContext _context;
    private readonly TimeProvider _time;

    public CrowdsourceRepository(GoTravelContext context, TimeProvider time)
    {
        _context = context;
        _time = time;
    }
    
    
    public async Task<IEnumerable<GTCrowdsourceInfo>> GetCrowdsourcesAndVotesForEntity(string entityId, bool includeReports = false, CancellationToken ct = default)
    {
        var infos = await _context.CrowdsourceInfo
            .Where(c => c.SubmittedAt < _time.GetUtcNow().UtcDateTime && c.ExpectedEnd > _time.GetUtcNow().UtcDateTime)
            .Include(c => c.Votes)
            .Include(c => c.SubmittedBy)
            .ToListAsync(ct);

        return infos;
    }

    public async Task SaveCrowdsource(GTCrowdsourceInfo info, CancellationToken ct = default)
    {
        if (_context.CrowdsourceInfo.AsNoTrackingWithIdentityResolution().Any(c => c.UUID == info.UUID))
        {
            _context.CrowdsourceInfo.Update(info);
        }
        else
        {
            _context.CrowdsourceInfo.Add(info);
        }

        await _context.SaveChangesAsync(ct);
    }

    public async Task<GTCrowdsourceInfo?> GetCrowdsource(string id, CancellationToken ct = default)
    {
        return await _context.CrowdsourceInfo
            .AsNoTrackingWithIdentityResolution()
            .Include(c => c.Votes)
            .Include(c => c.SubmittedBy)
            .FirstOrDefaultAsync(c => c.UUID == id, ct);
    }

    public async Task<GTCrowdsourceVotes?> GetVote(string crowdsourceId, string userId, CancellationToken ct = default)
    {
        return await _context.CrowdsourceVotes
            .AsNoTrackingWithIdentityResolution()
            .FirstOrDefaultAsync(v => v.CrowdsourceId == crowdsourceId && v.UserId == userId, ct);
    }

    public async Task<bool> DeleteVote(string crowdsourceId, string userId, CancellationToken ct = default)
    {
        var vote = await _context.CrowdsourceVotes
            .FirstOrDefaultAsync(v => v.CrowdsourceId == crowdsourceId && v.UserId == userId, ct);
        if (vote is not null)
        {
            _context.CrowdsourceVotes.Remove(vote);
        }

        return await _context.SaveChangesAsync(ct) > 0;
    }

    public async Task<bool> SaveVote(GTCrowdsourceVotes vote, CancellationToken ct = default)
    {
        if (_context.CrowdsourceVotes.AsNoTrackingWithIdentityResolution().Any(v => v.CrowdsourceId == vote.CrowdsourceId && v.UserId == vote.UserId))
        {
            _context.CrowdsourceVotes.Update(vote);
        }
        else
        {
            _context.CrowdsourceVotes.Add(vote);
        }

        return await _context.SaveChangesAsync(ct) > 0;
    }

    public async Task<bool> SaveReport(GTCrowdsourceReport report, CancellationToken ct = default)
    {
        if (_context.CrowdsourceReports.AsNoTrackingWithIdentityResolution().Any(r => r.UUID == report.UUID))
        {
            _context.CrowdsourceReports.Update(report);
        }
        else
        {
            _context.CrowdsourceReports.Add(report);
        }

        return await _context.SaveChangesAsync(ct) > 0;
    }
}