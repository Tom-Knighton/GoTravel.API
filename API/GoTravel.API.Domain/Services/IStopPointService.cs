using System.Collections;
using GoTravel.API.Domain.Models.DTOs;
using GoTravel.Standard.Models.Arrivals;
using GoTravel.Standard.Models.MessageModels;

namespace GoTravel.API.Domain.Services;

public interface IStopPointService
{
    /// <summary>
    /// Returns a list of stop points with a similar name to a search query.
    /// </summary>
    /// <param name="nameQuery">The search query</param>
    /// <param name="maxResults">Maximum number of results to return, defaults to 25</param>
    /// <param name="lineModeFilters">Filters the results by the given line modes</param>
    public Task<ICollection<StopPointBaseDto>> GetStopPointsByNameAsync(string nameQuery, ICollection<string> hiddenLineModes, int maxResults = 25, CancellationToken ct = default);
    
    /// <summary>
    /// Returns a list of stop points within a radius of a given point.
    /// </summary>
    /// <param name="latitude"></param>
    /// <param name="longitude"></param>
    /// <param name="radius">In physical metres, defaults to 850</param>
    /// <param name="maxResults">Maximum number of results to return, defaults to 25</param>
    /// <param name="lineModeFilters">Filters the results by the given line modes</param>
    /// <param name="ct"></param>
    public Task<ICollection<StopPointBaseDto>> GetStopPointsAroundPointAsync(float latitude, float longitude, ICollection<string> hiddenLineModes, int radius = 850, int maxResults = 25, CancellationToken ct = default);
    
    public Task<ICollection<StopPointBaseDto>> GetStopPointChildrenAsync(StopPointBaseDto stopPoint, CancellationToken ct = default);

    /// <summary>
    /// Consumes an update message and performs an UPSERT on some basic stop point information
    /// </summary>
    public Task UpdateStopPoint(StopPointUpdateDto update, CancellationToken ct = default);

    /// <summary>
    /// Returns the ids of stop points that are children of the provided id, or it's HUB id
    /// </summary>
    public Task<ICollection<string>> GetChildIdsAsync(string stopId, CancellationToken ct = default);
}