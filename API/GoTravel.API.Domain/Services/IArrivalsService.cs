using GoTravel.API.Domain.Models.DTOs;

namespace GoTravel.API.Domain.Services;

public interface IArrivalsService
{
    
    /// <summary>
    /// Retrieves arrivals for a specified stop point
    /// </summary>
    public Task<StopPointArrivalsDto> GetArrivalsForStopPointAsync(string stopId, bool includeChildrenAndHubs = false, CancellationToken ct = default);
}