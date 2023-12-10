using GoTravel.API.Domain.Models.Database;
using GoTravel.API.Domain.Models.DTOs;
using GoTravel.API.Domain.Services;
using GoTravel.API.Domain.Services.Mappers;
using GoTravel.API.Domain.Services.Repositories;
using NetTopologySuite.Geometries;

namespace GoTravel.API.Services.Services;

public class StopPointService: IStopPointService
{
    private readonly IStopPointRepository _repo;
    private readonly IMapper<GLStopPoint, StopPointBaseDto> _mapper;

    public StopPointService(IStopPointRepository repo, IMapper<GLStopPoint, StopPointBaseDto> mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }
    
    public async Task<ICollection<StopPointBaseDto>> GetStopPointsByNameAsync(string nameQuery, ICollection<string> hiddenLineModes, int maxResults = 25, CancellationToken ct = default)
    {
        var results = await _repo.GetStopPoints(nameQuery, maxResults, ct);

        var mapped = results.Select(s => _mapper.Map(s)).ToList();
        foreach (var stop in mapped)
        {
            stop.Children = await GetStopPointChildrenAsync(stop, ct);
        }
        
        if (hiddenLineModes.Any())
        {
            mapped = FilterLineModes(mapped, hiddenLineModes).ToList();
        }

        return mapped;
    }

    public async Task<ICollection<StopPointBaseDto>> GetStopPointsAroundPointAsync(float latitude, float longitude, ICollection<string> hiddenLineModes = null, int radius = 850, int maxResults = 25, CancellationToken ct = default)
    {
        var searchAroundPoint = new Point(longitude, latitude);
        var results = await _repo.GetStopPoints(searchAroundPoint, radius, maxResults, ct);

        var mapped = results.Select(s => _mapper.Map(s)).ToList();
        foreach (var stop in mapped)
        {
            stop.Children = await GetStopPointChildrenAsync(stop, ct);
        }

        if (hiddenLineModes.Any())
        {
            mapped = FilterLineModes(mapped, hiddenLineModes).ToList();
        }

        return mapped;
    }

    public async Task<ICollection<StopPointBaseDto>> GetStopPointChildrenAsync(StopPointBaseDto stopPoint, CancellationToken ct = default)
    {
        var children = await _repo.GetAllChildrenOf(stopPoint.StopPointId, ct);

        var mapped = children.Select(c => _mapper.Map(c)).ToList();
        foreach (var child in mapped)
        {
            child.Children = await GetStopPointChildrenAsync(child, ct);
        }
        
        return mapped;
    }

    /// <summary>
    /// Removes any line modes in the hiddenLineModes list.
    /// Removes any stop points that have no line modes left after filtering.
    /// </summary>
    private static IEnumerable<StopPointBaseDto> FilterLineModes(ICollection<StopPointBaseDto> stopPoints, ICollection<string> hiddenLineModes)
    {
        foreach (var stopPoint in stopPoints)
        {
            stopPoint.LineModes = stopPoint.LineModes.Where(lm => !hiddenLineModes.Contains(lm.LineModeName)).ToList();
            stopPoint.Children = FilterLineModes(stopPoint.Children, hiddenLineModes).ToList();
        }

        return stopPoints.Where(s => s.LineModes.Any());
    }
}