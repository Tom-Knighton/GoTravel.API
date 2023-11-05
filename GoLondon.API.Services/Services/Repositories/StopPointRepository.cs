using GoLondon.API.Domain.Data;
using GoLondon.API.Domain.Models.Database;
using GoLondon.API.Domain.Services.Repositories;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace GoLondon.API.Services.Services.Repositories;

public class StopPointRepository: IStopPointRepository
{
    private readonly GoLondonContext _context;

    public StopPointRepository(GoLondonContext context)
    {
        _context = context;
    }
    
    public async Task<ICollection<GLStopPoint>> GetStopPoints(string searchQuery, int maxResults, CancellationToken ct = default)
    {
        var results = await _context.StopPoints
            .Include(s => s.StopPointLines)
                .ThenInclude(l => l.Line)
                .ThenInclude(l => l.LineMode)
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
            .Where(s => s.StopPointParentId == stopPointId)
            .ToListAsync(cancellationToken: ct);

        return results;
    }
}