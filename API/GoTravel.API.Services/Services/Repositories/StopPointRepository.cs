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

    public async Task<bool> StopPointExists(string stopPointId, CancellationToken ct = default)
    {
        return await _context.StopPoints.AnyAsync(s => s.StopPointId == stopPointId, ct);
    }

    public async Task<ICollection<GTStopPoint>> GetStopPoints(string searchQuery, int maxResults, CancellationToken ct = default)
    {
        var results = await _context.StopPoints
            .IncludeLineHierarchy()
            .Where(s => EF.Functions.ILike(s.StopPointName, $"%{searchQuery}%") || s.BusStopSMSCode == searchQuery)
            .Take(maxResults)
            .ToListAsync(cancellationToken: ct);

        return results;
    }

    public async Task<ICollection<GTStopPoint>> GetStopPoints(Point searchPoint, int searchRadius, int maxResults, CancellationToken ct = default)
    {
        var results = await _context.StopPoints
            .IncludeLineHierarchy()
            .Where(s => EF.Functions.IsWithinDistance(searchPoint, s.StopPointCoordinate, searchRadius, true))
            .OrderBy(s => EF.Functions.Distance(searchPoint, s.StopPointCoordinate, true))
            .Take(maxResults)
            .ToListAsync(ct);

        return results;
    }

    public async Task<ICollection<GTStopPoint>> GetAllChildrenOf(string stopPointId, CancellationToken ct = default)
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

    public async Task<GTStopPoint?> GetStopPoint(string id, CancellationToken ct = default)
    {
        return await _context.StopPoints
            .IncludeLineHierarchy()
            .FirstOrDefaultAsync(s => s.StopPointId == id, cancellationToken: ct);
    }

    public async Task<GTStopPoint> Update(GTStopPoint stop, CancellationToken ct = default)
    {
        await _context.BulkInsertOrUpdateAsync(new List<GTStopPoint> { stop }, b =>
        {
            b.IncludeGraph = true;
        }, cancellationToken: ct);

        await _context.BulkSaveChangesAsync(cancellationToken: ct);

        return stop;
    }

    public async Task RemoveInfoValues(string stopPointId, CancellationToken ct = default)
    {
        var toDelete = await _context.StopPointInfoValues
            .Where(i => i.StopPointId == stopPointId)
            .ToListAsync(ct);
        await _context.BulkDeleteAsync(toDelete, cancellationToken: ct);
        await _context.BulkSaveChangesAsync(cancellationToken: ct);
    }

    public async Task InsertInfoValues(IEnumerable<GTStopPointInfoValue> values, CancellationToken ct = default)
    {
        var entities = values.ToList();
        await _context.BulkInsertOrUpdateAsync(entities, b =>
        {
            b.IncludeGraph = false;
        }, cancellationToken: ct);

        await _context.BulkSaveChangesAsync(cancellationToken: ct);
    }

    public async Task<ICollection<GTStopPointInfoValue>> GetInfoForStop(string id, CancellationToken ct = default)
    {
        return await _context.StopPointInfoValues
            .Where(i => i.StopPointId == id)
            .ToListAsync(ct);
    }

    public async Task<ICollection<GTStopPoint>> GetMinimumInfoFor(ICollection<string> ids, CancellationToken ct = default)
    {
        return await _context.StopPoints
            .Select(s => new { s.StopPointId, s.StopPointName, s.BusStopIndicator, s.BusStopLetter, s.StopPointCoordinate })
            .Where(s => ids.Contains(s.StopPointId))
            .Select(s => new GTStopPoint
            {
                StopPointName = s.StopPointName,
                StopPointId = s.StopPointId,
                BusStopIndicator = s.BusStopIndicator,
                BusStopLetter = s.BusStopLetter,
                StopPointCoordinate = s.StopPointCoordinate
            })
            .ToListAsync(ct);
    }
}

public static class StopPointRepositoryExtensions
{
    public static IQueryable<GTStopPoint> IncludeLineHierarchy(this IQueryable<GTStopPoint> query)
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