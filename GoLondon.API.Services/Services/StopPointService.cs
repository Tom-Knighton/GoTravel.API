using GoLondon.API.Domain.Data;
using GoLondon.API.Domain.Models.Database;
using GoLondon.API.Domain.Models.DTOs;
using GoLondon.API.Domain.Services;
using GoLondon.API.Domain.Services.Mappers;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace GoLondon.API.Services.Services;

public class StopPointService: IStopPointService
{
    private readonly GoLondonContext _context;
    private readonly IMapper<GLStopPoint, StopPointBaseDto> _mapper;

    public StopPointService(GoLondonContext db, IMapper<GLStopPoint, StopPointBaseDto> mapper)
    {
        _context = db;
        _mapper = mapper;
    }
    
    public async Task<ICollection<StopPointBaseDto>> GetStopPointsByNameAsync(string nameQuery, ICollection<string> filterLineModes, int maxResults = 25, CancellationToken ct = default)
    {
        var results = await _context.StopPoints
            .Include(s => s.StopPointLines)
                .ThenInclude(l => l.Line)
                    .ThenInclude(l => l.LineMode)
            .Where(s => EF.Functions.ILike(s.StopPointName, $"%{nameQuery}%") || s.BusStopSMSCode == nameQuery)
            .Take(maxResults)
            .ToListAsync(cancellationToken: ct);

        var mapped = results.Select(s => _mapper.Map(s)).ToList();
        foreach (var stop in mapped)
        {
            stop.Children = await GetStopPointChildrenAsync(stop, ct);
        }
        
        if (filterLineModes.Any())
        {
            mapped = FilterLineModes(mapped, filterLineModes).ToList();
        }

        return mapped;
    }

    public async Task<ICollection<StopPointBaseDto>> GetStopPointsAroundPointAsync(float latitude, float longitude, ICollection<string> filterLineModes = null, int radius = 850, int maxResults = 25, CancellationToken ct = default)
    {
        var searchAroundPoint = new Point(longitude, latitude);
        var results = await _context.StopPoints
            .Include(s => s.StopPointLines)
                .ThenInclude(l => l.Line)
                    .ThenInclude(l => l.LineMode)
            .Where(s => EF.Functions.IsWithinDistance(searchAroundPoint, s.StopPointCoordinate, radius, true))
            .OrderBy(s => EF.Functions.Distance(searchAroundPoint, s.StopPointCoordinate, true))
            .Take(maxResults)
            .ToListAsync(ct);

        var mapped = results.Select(s => _mapper.Map(s)).ToList();
        foreach (var stop in mapped)
        {
            stop.Children = await GetStopPointChildrenAsync(stop, ct);
        }

        if (filterLineModes.Any())
        {
            mapped = FilterLineModes(mapped, filterLineModes).ToList();
        }

        return mapped;
    }

    public async Task<ICollection<StopPointBaseDto>> GetStopPointChildrenAsync(StopPointBaseDto stopPoint, CancellationToken ct = default)
    {
        var children = await _context.StopPoints
            .Include(s => s.StopPointLines)
                .ThenInclude(l => l.Line)
                    .ThenInclude(l => l.LineMode)
            .Where(s => s.StopPointParentId == stopPoint.StopPointId)
            .ToListAsync(cancellationToken: ct);

        var mapped = children.Select(c => _mapper.Map(c)).ToList();
        foreach (var child in mapped)
        {
            child.Children = await GetStopPointChildrenAsync(child, ct);
        }
        
        return mapped;
    }

    /// <summary>
    /// Removes any line modes that are not in the filterLineModes list.
    /// Removes any stop points that have no line modes left after filtering.
    /// </summary>
    private static IEnumerable<StopPointBaseDto> FilterLineModes(ICollection<StopPointBaseDto> stopPoints, ICollection<string> filterLineModes)
    {
        foreach (var stopPoint in stopPoints)
        {
            stopPoint.LineModes = stopPoint.LineModes.Where(lm => filterLineModes.Contains(lm.LineModeName)).ToList();
            stopPoint.Children = FilterLineModes(stopPoint.Children, filterLineModes).ToList();
        }

        return stopPoints.Where(s => s.LineModes.Any());
    }
}