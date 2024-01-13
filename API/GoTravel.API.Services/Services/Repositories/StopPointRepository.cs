using EFCore.BulkExtensions;
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

    public async Task<ICollection<string>> GetChildIdsOf(string stopPointId, CancellationToken ct = default)
    {
        var results = await _context.StopPoints
            .Where(s => s.StopPointParentId == stopPointId)
            .Select(s => s.StopPointId)
            .ToListAsync(ct);

        return results;
    }

    public async Task<ICollection<string>> GetIdsOfStopsAtHub(string hubId, CancellationToken ct = default)
    {
        var results = await _context.StopPoints
            .Where(s => s.StopPointHub == hubId)
            .Select(s => s.StopPointId)
            .ToListAsync(ct);

        return results;
    }

    public async Task<GLStopPoint?> GetStopPoint(string id, CancellationToken ct = default)
    {
        return await _context.StopPoints
            .IncludeLineHierarchy()
            .FirstOrDefaultAsync(s => s.StopPointId == id, cancellationToken: ct);
    }

    public async Task<GLStopPoint> Update(GLStopPoint stop, CancellationToken ct = default)
    {
        await _context.BulkInsertOrUpdateAsync(new List<GLStopPoint> { stop }, b =>
        {
            b.IncludeGraph = true;
        }, cancellationToken: ct);

        await _context.BulkSaveChangesAsync(cancellationToken: ct);

        return stop;
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