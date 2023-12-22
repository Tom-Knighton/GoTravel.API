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
    
    public async Task<ICollection<GLLineMode>> GetLineModes(CancellationToken ct = default)
    {
        return await _context.LineModes
            .Include(lm => lm.Lines)
            .Include(lm => lm.PrimaryArea)
            .Include(lm => lm.Flags)
            .ToListAsync(ct);
    }
}