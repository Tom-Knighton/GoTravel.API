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
    
    
    public async Task<IEnumerable<GTCrowdsourceInfo>> GetCrowdsourcesAndVotesForEntity(string entityId, CancellationToken ct = default)
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
}