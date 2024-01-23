using System.Text.Json;
using GoTravel.API.Domain.Models.DTOs;
using GoTravel.API.Domain.Services;
using GoTravel.API.Domain.Services.Mappers;
using GoTravel.API.Domain.Singletons;
using GoTravel.Standard.Models.Arrivals;
using NRedisStack.RedisStackCommands;
using StackExchange.Redis;

namespace GoTravel.API.Services.Services;

public class ArrivalsService: IArrivalsService
{
    private readonly IStopPointService _stopPointService;
    private readonly HttpClient _connector;
    private readonly IDatabase _cache;
    private readonly IMapper<IEnumerable<ArrivalDeparture>, ICollection<LineArrivals>> _mapper;

    public ArrivalsService(IStopPointService stopPointService, IHttpClientFactory httpClients, IDatabase db, IMapper<IEnumerable<ArrivalDeparture>, ICollection<LineArrivals>> map)
    {
        _stopPointService = stopPointService;
        _connector = httpClients.CreateClient("GTCON");
        _cache = db;
        _mapper = map;
    }
    
    public async Task<StopPointArrivalsDto> GetArrivalsForStopPointAsync(string stopId, bool includeChildrenAndHubs = false, CancellationToken ct = default)
    {
        var cacheKey = $"cache_arrivals:{stopId}_{(includeChildrenAndHubs ? "_withChildren" : "")}";
        if (await _cache.KeyExistsAsync(cacheKey))
        {
            var cached = await _cache.JSON().GetAsync<StopPointArrivalsDto>(cacheKey);
            if (cached is not null)
            {
                return cached;
            }
        }

        var dto = new StopPointArrivalsDto
        {
            StopPointId = stopId,
            ModeArrivals = new List<StopPointArrivalsMode>()
        };
        
        var stopIds = new List<string> { stopId };
        if (includeChildrenAndHubs)
        {
            stopIds.AddRange(await _stopPointService.GetChildIdsAsync(stopId, ct));
        }

        var arrivalTasks = stopIds.Distinct().Select(id => _connector.GetStreamAsync($"Arrivals/{id}", ct)).ToList();
        var getAllArrivalTasks = Task.WhenAll(arrivalTasks);
        try
        {
            await getAllArrivalTasks;
        }
        catch (Exception ex)
        {
            //TODO: log
            Console.WriteLine("Arrival tasks falted");
        }

        var decodeResults = arrivalTasks
            .Where(t => t.IsCompletedSuccessfully)
            .Select(async t => await JsonSerializer.DeserializeAsync<ICollection<ArrivalDeparture>>(t.Result, JsonSingleton.Options, cancellationToken: ct));
        var flatArrivals = (await Task.WhenAll(decodeResults)).SelectMany(r => r ?? new List<ArrivalDeparture>());

        var grouped = _mapper.Map(flatArrivals);

        foreach (var mode in grouped.GroupBy(x => x.LineMode))
        {
            dto.ModeArrivals.Add(new StopPointArrivalsMode
            {
                ModeId = mode.Key,
                LineArrivals = mode.ToList()
            });
        }

        await _cache.JSON().SetAsync(cacheKey, "$", dto);
        await _cache.KeyExpireAsync(cacheKey, new TimeSpan(0, 0, 0, 30));
        
        return dto;
    }
}