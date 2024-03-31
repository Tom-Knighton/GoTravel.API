using GoTravel.API.Domain.Models.Database;
using GoTravel.API.Domain.Models.DTOs;
using GoTravel.API.Domain.Services;
using GoTravel.API.Domain.Services.Mappers;
using GoTravel.API.Domain.Services.Repositories;
using GoTravel.Standard.Models.MessageModels;
using NetTopologySuite.Geometries;

namespace GoTravel.API.Services.Services;

public class LineModeService: ILineModeService
{
    private readonly ILineModeRepository _repo;
    private readonly IAreaRepository _areaRepo;
    private readonly IMapper<GLLineMode, LineModeDto> _mapper;
    private readonly IMapper<GTLine, LineDto> _lineMap;

    public LineModeService(ILineModeRepository repo, IAreaRepository areas, IMapper<GLLineMode, LineModeDto> map, IMapper<GTLine, LineDto> lineMap)
    {
        _repo = repo;
        _areaRepo = areas;
        _mapper = map;
        _lineMap = lineMap;
    }
    
    public async Task<IEnumerable<LineModeSearchResult>> ListAsync(float? searchLatitude, float? searchLongitude, CancellationToken ct = default)
    {
        var results = await _repo.GetLineModes(ct: ct);
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

    public async Task<IEnumerable<LineModeDto>> ListFromLineIdsAsync(ICollection<string> lineIds, CancellationToken ct = default)
    {
        var results = await _repo.GetLineModesByLineIds(lineIds, false, ct);
        var dtos = results.Select(r => _mapper.Map(r)).ToList();
        return dtos;
    }

    public async Task<string> GetAreaNameFromCoordinates(float latitude, float longitude, CancellationToken ct = default)
    {
        var area = await _areaRepo.GetAreaFromPoint(new Point(longitude, latitude) { SRID = 4326 }, ct);
        
        return area?.AreaName ?? "UK";
    }

    public async Task UpdateLineMode(LineModeUpdateDto update, CancellationToken ct = default)
    {
        var existing = await _repo.GetLineMode(update.LineModeName, true, ct: ct);

        // If mode is new, create a new GLLineMode
        if (existing is null)
        {
            existing = new GLLineMode
            {
                LineModeId = update.LineModeName,
                LineModeName = "",
                IsEnabled = false,
                PrimaryColour = "",
                LogoUrl = "",
                BrandingColour = "",
            };
        }

        // Get all new lines, that exist in update but not in DB
        var newLines = update.Lines?.Where(nl => existing.Lines.All(l => l.LineId != nl)) ?? new List<string>();

        var lines = newLines.Select(l => new GTLine
        {
            LineId = l,
            LineName = string.Empty,
            IsEnabled = false,
            LineModeId = existing.LineModeId
        });
        
        foreach (var newLine in lines)
        {
            existing.Lines.Add(newLine);
        }

        await _repo.Update(existing, ct);
    }

    public async Task UpdateLineRoute(LineStringUpdateDto update, CancellationToken ct = default)
    {
        var lineStrings = update.Route
            .Select(r => new LineString(r.Select(p => new Coordinate(p.ElementAt(0), p.ElementAt(1))).ToArray()))
            .ToArray();
        var route = new GTLineRoute
        {
            LineId = update.LineId,
            Name = update.Name,
            ServiceType = update.Service,
            Direction = update.Direction,
            Route = new MultiLineString(lineStrings)
        };
        
        await _repo.UpdateRoute(route, ct);
    }

    public async Task<ICollection<LineDto>> SearchByName(string query, int maxResults, CancellationToken ct = default)
    {
        var lines = await _repo.GetByName(query, maxResults, false, ct);
        var dtos = lines.Select(l => _lineMap.Map(l));

        return dtos.ToList();
    }
}