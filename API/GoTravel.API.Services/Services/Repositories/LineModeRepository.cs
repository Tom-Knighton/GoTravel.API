using EFCore.BulkExtensions;
using GoTravel.API.Domain.Data;
using GoTravel.API.Domain.Models.Database;
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
}