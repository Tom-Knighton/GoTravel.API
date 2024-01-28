using GoTravel.API.Domain.Models.DTOs;
using GoTravel.Standard.Models.MessageModels;

namespace GoTravel.API.Domain.Services;

public interface ILineModeService
{
    public Task<IEnumerable<LineModeSearchResult>> ListAsync(float? searchLatitude, float? searchLongitude, CancellationToken ct = default);
    public Task<IEnumerable<LineModeDto>> ListFromLineIdsAsync(ICollection<string> lineIds, CancellationToken ct = default);
    public Task<string> GetAreaNameFromCoordinates(float latitude, float longitude, CancellationToken ct = default);

    public Task UpdateLineMode(LineModeUpdateDto update, CancellationToken ct = default);
}