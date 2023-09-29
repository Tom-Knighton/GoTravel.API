using GoLondon.API.Domain.Data;
using GoLondon.API.Domain.Models.Database;
using GoLondon.API.Domain.Models.DTOs;
using GoLondon.API.Domain.Services;
using GoLondon.API.Domain.Services.Mappers;
using Microsoft.EntityFrameworkCore;

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
    
    public async Task<ICollection<StopPointBaseDto>> GetStopPointsByNameAsync(string nameQuery, CancellationToken ct = default)
    {
        var results = await _context.StopPoints
            .Include(s => s.StopPointLines)
                .ThenInclude(l => l.Line)
                    .ThenInclude(l => l.LineMode)
            .Where(s => s.StopPointName.Contains(nameQuery, StringComparison.InvariantCultureIgnoreCase))
            .ToListAsync(cancellationToken: ct);

        var mapped = results.Select(s => _mapper.Map(s)).ToList();
        foreach (var stop in mapped)
        {
            stop.Children = await GetStopPointChildrenAsync(stop, ct);
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
}