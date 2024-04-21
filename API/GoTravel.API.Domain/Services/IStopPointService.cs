using GoTravel.API.Domain.Models.Database;
using GoTravel.API.Domain.Models.DTOs;
using GoTravel.Standard.Models;
using GoTravel.Standard.Models.MessageModels;

namespace GoTravel.API.Domain.Services;

public interface IStopPointService
{
    /// <summary>
    /// Returns a stop point from a given id, optionally returning the whole HUB.
    /// </summary>
    /// <param name="stopId">The id of the stop point to get details for</param>
    /// <param name="getHub">Whether or not to return the entire HUB rather than the stop itself</param>
    public Task<StopPointBaseDto?> GetStopPoint(string stopId, bool getHub = false, CancellationToken ct = default);
    
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

    /// <summary>
    /// Removes all info values from the stop point, and adds the one in the update message
    /// </summary>
    public Task ClearAndUpdateStopPointInfo(string stopId, ICollection<KeyValuePair<StopPointInfoKey, string>> infoKvps, CancellationToken ct = default);

    /// <summary>
    /// Returns a dto of stop point information for a specific stop
    /// </summary>
    /// <param name="stopId">The id of the stop to request information for</param>
    /// <param name="useHubOrParent">Whether or not to use the Hub or parent's info instead of the specific stop</param>
    public Task<StopPointInformationDto> GetStopPointInformation(string stopId, bool useHubOrParent = false, CancellationToken ct = default);

    /// <summary>
    /// Returns super basic information about stops (i.e. names) from a list of ids, mainly used for journeys
    /// </summary>
    /// <param name="stopIds">The stop ids to request</param>
    public Task<ICollection<JourneyLegStopPointDto>> GetBasicLegStopPointDtos(ICollection<string> stopIds, CancellationToken ct = default);

    /// <summary>
    /// Returns all stop points, paginated
    /// </summary>
    public Task<ICollection<GTStopPoint>> RetrievePaginated(string? query, int results, int startFrom, CancellationToken ct = default);

    /// <summary>
    /// Retrieves the raw info k/vs as a list of value objects for a stop
    /// </summary>
    public Task<ICollection<GTStopPointInfoValue>> GetStopPointInfoKvs(string stopId, CancellationToken ct = default);

    /// <summary>
    /// Retrieves a GTStopPoint model
    /// </summary>
    public Task<GTStopPoint?> GetGTStop(string stopId, CancellationToken ct = default);
}