using GoTravel.API.Domain.Data;
using GoTravel.API.Domain.Models.Database;
using GoTravel.API.Domain.Services.Repositories;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace GoTravel.API.Services.Services.Repositories;

public class AreaRepository: IAreaRepository
{
    private readonly GoTravelContext _context;

    public AreaRepository(GoTravelContext context)
    {
        _context = context;
    }
    
    public async Task<GTArea?> GetAreaFromPoint(Point point, CancellationToken ct = default)
    {
        return await _context.Areas
            .FirstOrDefaultAsync(a => a.AreaCatchment.Contains(point), ct);
    }
}