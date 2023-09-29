using GoLondon.API.Domain.Models.DTOs;

namespace GoLondon.API.Domain.Services;

public interface IStopPointService
{
    public Task<ICollection<StopPointBaseDto>> GetStopPointsByNameAsync(string nameQuery, int maxResults = 25, CancellationToken ct = default);

    public Task<ICollection<StopPointBaseDto>> GetStopPointChildrenAsync(StopPointBaseDto stopPoint, CancellationToken ct = default);
}