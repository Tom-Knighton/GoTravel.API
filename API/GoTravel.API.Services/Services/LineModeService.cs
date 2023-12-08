using GoTravel.API.Domain.Models.Database;
using GoTravel.API.Domain.Models.DTOs;
using GoTravel.API.Domain.Services;
using GoTravel.API.Domain.Services.Mappers;
using GoTravel.API.Domain.Services.Repositories;
using NetTopologySuite.Geometries;

namespace GoTravel.API.Services.Services;

public class LineModeService: ILineModeService
{
    private readonly ILineModeRepository _repo;
    private readonly IAreaRepository _areaRepo;
    private readonly IMapper<GLLineMode, LineModeDto> _mapper;

    public LineModeService(ILineModeRepository repo, IAreaRepository areas, IMapper<GLLineMode, LineModeDto> map)
    {
        _repo = repo;
        _areaRepo = areas;
        _mapper = map;
    }
    
    public async Task<IEnumerable<LineModeSearchResult>> ListAsync(float? searchLatitude, float? searchLongitude, CancellationToken ct = default)
    {
        var results = await _repo.GetLineModes(ct);
        var mapped = results.Select(_mapper.Map).ToList();

        var grouped = mapped.GroupBy(lm => lm.PrimaryAreaName);
        var dtos = grouped.Select(g => new LineModeSearchResult
        {
            AreaName = g.Key,
            LineModes = g.ToList()
        });
        
        if (searchLatitude.HasValue && searchLongitude.HasValue)
        {
            var searchPoint = new Point(searchLongitude.Value, searchLatitude.Value) { SRID = 4326 };
            var area = await _areaRepo.GetAreaFromPoint(searchPoint, ct);
            dtos = dtos.OrderByDescending(g => g.AreaName == area?.AreaName);
        }

        return dtos;
    }

    public async Task<string> GetAreaNameFromCoordinates(float latitude, float longitude, CancellationToken ct = default)
    {
        var area = await _areaRepo.GetAreaFromPoint(new Point(longitude, latitude) { SRID = 4326 }, ct);
        
        return area?.AreaName ?? "UK";
    }
}