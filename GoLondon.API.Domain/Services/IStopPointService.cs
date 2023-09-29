using GoLondon.API.Domain.Models.DTOs;

namespace GoLondon.API.Domain.Services;

public interface IStopPointService
{
    /// <summary>
    /// Returns a list of stop points with a similar name to a search query.
    /// </summary>
    public Task<ICollection<StopPointBaseDto>> GetStopPointsByNameAsync(string nameQuery, int maxResults = 25, CancellationToken ct = default);
    
    /// <summary>
    /// Returns a list of stop points within a radius of a given point.
    /// </summary>
    /// <param name="latitude"></param>
    /// <param name="longitude"></param>
    /// <param name="radius">In physical metres, defaults to 850</param>
    /// <param name="maxResults">Maximum number of results to return, defaults to 25</param>
    /// <param name="ct"></param>
    public Task<ICollection<StopPointBaseDto>> GetStopPointsAroundPointAsync(float latitude, float longitude, int radius = 850, int maxResults = 25, CancellationToken ct = default);
    
    public Task<ICollection<StopPointBaseDto>> GetStopPointChildrenAsync(StopPointBaseDto stopPoint, CancellationToken ct = default);
 }