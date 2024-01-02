using System.Linq.Expressions;
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
            .IncludeLineHierarchy()
            .Where(s => EF.Functions.ILike(s.StopPointName, $"%{searchQuery}%") || s.BusStopSMSCode == searchQuery)
            .Take(maxResults)
            .ToListAsync(cancellationToken: ct);

        return results;
    }

    public async Task<ICollection<GLStopPoint>> GetStopPoints(Point searchPoint, int searchRadius, int maxResults, CancellationToken ct = default)
    {
        var results = await _context.StopPoints
            .IncludeLineHierarchy()
            .Where(s => EF.Functions.IsWithinDistance(searchPoint, s.StopPointCoordinate, searchRadius, true))
            .OrderBy(s => EF.Functions.Distance(searchPoint, s.StopPointCoordinate, true))
            .Take(maxResults)
            .ToListAsync(ct);

        return results;
    }

    public async Task<ICollection<GLStopPoint>> GetAllChildrenOf(string stopPointId, CancellationToken ct = default)
    {
        var results = await _context.StopPoints
            .IncludeLineHierarchy()
            .Where(s => s.StopPointParentId == stopPointId)
            .AsSingleQuery()
            .ToListAsync(cancellationToken: ct);

        return results;
    }
}

public static class StopPointRepositoryExtensions
{
    public static IQueryable<GLStopPoint> IncludeLineHierarchy(this IQueryable<GLStopPoint> query)
    {
        return query
            .Include(x => x.StopPointLines.Where(l => l.IsEnabled))
                .ThenInclude(l => l.Line)
                    .ThenInclude(l => l.LineMode)
                        .ThenInclude(lm => lm.PrimaryArea)
            .Include(x => x.StopPointLines.Where(l => l.IsEnabled))
                .ThenInclude(l => l.Line)
                    .ThenInclude(l => l.LineMode)
                        .ThenInclude(lm => lm.Flags);
    }
}