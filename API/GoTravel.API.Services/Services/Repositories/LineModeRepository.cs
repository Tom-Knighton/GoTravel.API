using EFCore.BulkExtensions;
using GoTravel.API.Domain.Data;
using GoTravel.API.Domain.Models.Database;
using GoTravel.API.Domain.Models.DTOs;
using GoTravel.API.Domain.Services.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GoTravel.API.Services.Services.Repositories;

public class LineModeRepository: ILineModeRepository
{
    private readonly GoTravelContext _context;

    public LineModeRepository(GoTravelContext context)
    {
        _context = context;
    } 
    
    public async Task<ICollection<GLLineMode>> GetLineModes(bool includeDisabled = false, CancellationToken ct = default)
    {
        return await _context.LineModes
            .Where(lm => includeDisabled || lm.IsEnabled)
            .Include(lm => lm.Lines.Where(l => includeDisabled || l.IsEnabled))
            .Include(lm => lm.PrimaryArea)
            .Include(lm => lm.Flags)
            .ToListAsync(ct);
    }

    public async Task<GLLineMode?> GetLineMode(string id, bool includeDisabled = false, CancellationToken ct = default)
    {
        return await _context.LineModes
            .Include(lm => lm.Lines.Where(l => includeDisabled || l.IsEnabled))
            .Include(lm => lm.PrimaryArea)
            .Include(lm => lm.Flags)
            .FirstOrDefaultAsync(l => l.LineModeId == id, ct);
    }

    public async Task<ICollection<GLLineMode>> GetLineModesByLineIds(ICollection<string> lineIds, bool includeDisabled = false, CancellationToken ct = default)
    {
        return await _context.LineModes
            .Include(lm => lm.Lines.Where(l => lineIds.Contains(l.LineId) && (includeDisabled || l.IsEnabled)))
            .Include(lm => lm.PrimaryArea)
            .Include(lm => lm.Flags)
            .Where(lm => lm.Lines.Any(l => lineIds.Contains(l.LineId)))
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<GLLineMode> Update(GLLineMode mode, CancellationToken ct = default)
    {
        await _context.BulkInsertOrUpdateAsync(new List<GLLineMode> { mode }, b =>
        {
            b.IncludeGraph = true;
            b.PropertiesToExcludeOnUpdate = new List<string> { "IsEnabled" };
        }, cancellationToken: ct);

        await _context.BulkSaveChangesAsync(cancellationToken: ct);

        return mode;
    }

    public async Task<ICollection<GTLineRoute>> GetRoutesForLine(string lineId, CancellationToken ct = default)
    {
        return await _context.LineRoutes
            .Where(r => r.LineId == lineId)
            .ToListAsync(ct);
    }

    public async Task<GTLineRoute> UpdateRoute(GTLineRoute route, CancellationToken ct = default)
    {
        await _context.BulkInsertOrUpdateAsync(new List<GTLineRoute> { route }, b =>
        {
            b.IncludeGraph = true;
        }, cancellationToken: ct);

        await _context.BulkSaveChangesAsync(cancellationToken: ct);

        return route;
    }

    public async Task<ICollection<GTLine>> GetByName(string name, int maxResults, bool includeDisabled = false, CancellationToken ct = default)
    {
        var lines = await _context.Lines
            .Where(l => EF.Functions.ILike(l.LineName, $"%{name}%") && (includeDisabled || l.IsEnabled))
            .Take(maxResults)
            .ToListAsync(ct);

        return lines;
    }
}