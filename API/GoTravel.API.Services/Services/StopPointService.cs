using GoTravel.API.Domain.Models.Database;
using GoTravel.API.Domain.Models.DTOs;
using GoTravel.API.Domain.Services;
using GoTravel.API.Domain.Services.Mappers;
using GoTravel.API.Domain.Services.Repositories;
using GoTravel.Standard.Models.MessageModels;
using NetTopologySuite.Geometries;

namespace GoTravel.API.Services.Services;

public class StopPointService: IStopPointService
{
    private readonly IStopPointRepository _repo;
    private readonly IMapper<GLStopPoint, StopPointBaseDto> _mapper;
    private readonly IMapper<StopPointUpdateDto, GLStopPoint> _updateMapper;

    public StopPointService(IStopPointRepository repo, IMapper<GLStopPoint, StopPointBaseDto> mapper, IMapper<StopPointUpdateDto, GLStopPoint> updateMap)
    {
        _repo = repo;
        _mapper = mapper;
        _updateMapper = updateMap;
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

    public async Task UpdateStopPoint(StopPointUpdateDto update, CancellationToken ct = default)
    {

        var stopPoint = await _repo.GetStopPoint(update.Id, ct) ?? _updateMapper.Map(update);

        var updatedLines = update.Lines?
            .Where(l => stopPoint.StopPointLines.Select(x => x.LineId).Contains(l)) ?? new List<string>();
        
        if (update.Latitude is not null && update.Longitude is not null)
        {
            stopPoint.StopPointCoordinate = new Point(update.Longitude ?? 0, update.Latitude ?? 0);
        }

        stopPoint.BusStopIndicator = update.Indicator ?? stopPoint.BusStopIndicator;
        stopPoint.BusStopLetter = update.Letter ?? stopPoint.BusStopLetter;
        stopPoint.StopPointHub = update.HubId ?? stopPoint.StopPointHub;
        stopPoint.StopPointName = update.Name ?? stopPoint.StopPointName;
        stopPoint.BusStopSMSCode = update.SMS ?? stopPoint.BusStopSMSCode;
        stopPoint.StopPointParentId = update.ParentId ?? stopPoint.StopPointParentId;
        
        foreach (var lineToAdd in updatedLines)
        {
            stopPoint.StopPointLines.Add(new GLStopPointLine
            {
                LineId = lineToAdd,
                IsEnabled = false,
                StopPointId = stopPoint.StopPointId
            });
        }

        await _repo.Update(stopPoint, ct);
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