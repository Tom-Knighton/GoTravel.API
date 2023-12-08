using GoTravel.API.Domain.Models.DTOs;

namespace GoTravel.API.Domain.Services;

public interface ILineModeService
{
    Task<IEnumerable<LineModeSearchResult>> ListAsync(float? searchLatitude, float? searchLongitude, CancellationToken ct = default);
    Task<string> GetAreaNameFromCoordinates(float latitude, float longitude, CancellationToken ct = default);
}