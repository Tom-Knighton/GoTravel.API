using System.Text;
using GoTravel.API.Domain.Exceptions;
using GoTravel.API.Domain.Models.Database;
using GoTravel.API.Domain.Models.DTOs;
using GoTravel.API.Domain.Services;
using GoTravel.API.Domain.Services.Mappers;
using GoTravel.API.Domain.Services.Repositories;
using GoTravel.Standard.Models;
using GoTravel.Standard.Models.MessageModels;
using NetTopologySuite.Geometries;
using NRedisStack.RedisStackCommands;
using StackExchange.Redis;

namespace GoTravel.API.Services.Services;

public class StopPointService: IStopPointService
{
    private readonly IStopPointRepository _repo;
    private readonly IMapper<GTStopPoint, StopPointBaseDto> _mapper;
    private readonly IMapper<StopPointUpdateDto, GTStopPoint> _updateMapper;
    private readonly IMapper<ICollection<GTStopPointInfoValue>, StopPointInformationDto> _infoMapper;
    private readonly IDatabase _cache;

    public StopPointService(IStopPointRepository repo, IMapper<GTStopPoint, StopPointBaseDto> mapper, IMapper<StopPointUpdateDto, GTStopPoint> updateMap,
        IMapper<ICollection<GTStopPointInfoValue>, StopPointInformationDto> infoMapper, IDatabase db)
    {
        _repo = repo;
        _mapper = mapper;
        _updateMapper = updateMap;
        _infoMapper = infoMapper;
        _cache = db;
    }

    public async Task<StopPointBaseDto?> GetStopPoint(string stopId, bool getHub = false, CancellationToken ct = default)
    {
        var result = await _repo.GetStopPoint(stopId, ct);

        if (result is null)
        {
            throw new NoStopPointException(stopId);
        }

        if (!string.IsNullOrWhiteSpace(result.StopPointHub) && getHub)
        {
            result = await _repo.GetStopPoint(result.StopPointHub, ct) ?? result;
        }

        var dto = _mapper.Map(result);

        return dto;
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

    public async Task<ICollection<string>> GetChildIdsAsync(string stopId, CancellationToken ct = default)
    {
        var allIds = new List<string>();

        var stop = await _repo.GetStopPoint(stopId, ct);
        if (stop is null)
            return allIds;

        if (!string.IsNullOrWhiteSpace(stop.StopPointHub))
        {
            allIds.AddRange(await _repo.GetIdsOfStopsAtHub(stop.StopPointHub, ct));
        }

        await GetChildIdsRecursive(stopId, allIds);

        return allIds.Distinct().ToList();
    }

    public async Task ClearAndUpdateStopPointInfo(string stopId, ICollection<KeyValuePair<StopPointInfoKey, string>> infoKvps, CancellationToken ct = default)
    {
        if (await _repo.StopPointExists(stopId, ct) == false)
        {
            //TODO: Log
            Console.WriteLine("TRIED TO WRITE INFO FOR STOP POINT WHICH DOESNT EXIST");
            return;
        }
        
        
        await _repo.RemoveInfoValues(stopId, ct);

        var newValues = new List<GTStopPointInfoValue>();
        foreach (var kvp in infoKvps)
        {
            newValues.Add(new GTStopPointInfoValue
            {
                KeyId = kvp.Key,
                StopPointId = stopId,
                Value = kvp.Value
            });
        }

        await _repo.InsertInfoValues(newValues, ct);
    }

    public async Task<StopPointInformationDto> GetStopPointInformation(string stopId, bool useHub = false, CancellationToken ct = default)
    {
        var cacheKey = $"cache_info:{stopId}_{(useHub ? "hub" : "self")}_information";
        if (await _cache.KeyExistsAsync(cacheKey))
        {
            var cached = await _cache.JSON().GetAsync<StopPointInformationDto>(cacheKey);
            if (cached is not null)
            {
                return cached;
            }
        }
        
        var stopPoint = await _repo.GetStopPoint(stopId, ct) ?? throw new NoStopPointException(stopId);

        if (useHub && !string.IsNullOrWhiteSpace(stopPoint.StopPointHub))
        {
            stopPoint = await _repo.GetStopPoint(stopPoint.StopPointHub, ct) ?? throw new NoStopPointException(stopPoint.StopPointHub);
        }

        var info = await _repo.GetInfoForStop(stopPoint.StopPointId, ct);
        var dto = _infoMapper.Map(info);

        if (dto is not null)
        {
            await _cache.JSON().SetAsync(cacheKey, "$", dto);
            await _cache.KeyExpireAsync(cacheKey, new TimeSpan(1, 0, 0, 0));
        }

        return dto;
    }

    public async Task<ICollection<JourneyLegStopPointDto>> GetBasicLegStopPointDtos(ICollection<string> stopIds, CancellationToken ct = default)
    {
        var stops = await _repo.GetMinimumInfoFor(stopIds, ct);
        var dtos = stops.Select(s =>
        {
            var suffix = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(s.BusStopLetter))
            {
                suffix.Append($" (Stop {s.BusStopLetter})");
            }
            else if (!string.IsNullOrWhiteSpace(s.BusStopIndicator))
            {
                suffix.Append($" ({s.BusStopIndicator})");
            }

            return new JourneyLegStopPointDto
            {
                StopPointId = s.StopPointId,
                StopPointName = s.StopPointName + suffix,
                StopCoordinate = s.StopPointCoordinate
            };
        });

        var ordered = stopIds
            .Select(id => dtos.FirstOrDefault(s => s.StopPointId == id))
            .Where(s => s is not null)
            .ToList();

        return ordered;
    }

    private async Task GetChildIdsRecursive(string id, ICollection<string> results)
    {
        results.Add(id);

        var children = await _repo.GetChildIdsOf(id);

        foreach (var child in children)
        {
            await GetChildIdsRecursive(child, results);
        }
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