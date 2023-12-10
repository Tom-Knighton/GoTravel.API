using GoTravel.API.Domain.Data;
using GoTravel.API.Domain.Models.Database;
using GoTravel.API.Domain.Services.Repositories;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace GoTravel.API.Services.Services.Repositories;

public class StopPointRepository: IStopPointRepository
{
    private readonly GoTravelContext _context;

    public StopPointRepository(GoTravelContext context)
    {
        _context = context;
    }
    
    public async Task<ICollection<GLStopPoint>> GetStopPoints(string searchQuery, int maxResults, CancellationToken ct = default)
    {
        var results = await _context.StopPoints
            .Include(s => s.StopPointLines)
                .ThenInclude(l => l.Line)
                    .ThenInclude(l => l.LineMode)
                        .ThenInclude(lm => lm.PrimaryArea)
            .Where(s => EF.Functions.ILike(s.StopPointName, $"%{searchQuery}%") || s.BusStopSMSCode == searchQuery)
            .Take(maxResults)
            .ToListAsync(cancellationToken: ct);

        return results;
    }

    public async Task<ICollection<GLStopPoint>> GetStopPoints(Point searchPoint, int searchRadius, int maxResults, CancellationToken ct = default)
    {
        var results = await _context.StopPoints
            .Include(s => s.StopPointLines)
                .ThenInclude(l => l.Line)
                    .ThenInclude(l => l.LineMode)
                        .ThenInclude(lm => lm.PrimaryArea)
            .Where(s => EF.Functions.IsWithinDistance(searchPoint, s.StopPointCoordinate, searchRadius, true))
            .OrderBy(s => EF.Functions.Distance(searchPoint, s.StopPointCoordinate, true))
            .Take(maxResults)
            .ToListAsync(ct);

        return results;
    }

    public async Task<ICollection<GLStopPoint>> GetAllChildrenOf(string stopPointId, CancellationToken ct = default)
    {
        var results = await _context.StopPoints
            .Include(s => s.StopPointLines)
                .ThenInclude(l => l.Line)
                    .ThenInclude(l => l.LineMode)
                        .ThenInclude(lm => lm.PrimaryArea)
            .Where(s => s.StopPointParentId == stopPointId)
            .ToListAsync(cancellationToken: ct);

        return results;
    }
}