using System.Collections;
using GoTravel.API.Domain.Models.Database;
using GoTravel.Standard.Models;
using NetTopologySuite.Geometries;

namespace GoTravel.API.Domain.Services.Repositories;

public interface IStopPointRepository
{

    /// <summary>
    /// Whether or not a stop point exists with the specified id
    /// </summary>
    public Task<bool> StopPointExists(string stopPointId, CancellationToken ct = default);
    
    /// <summary>
    /// Returns a list of StopPoints, their lines and line modes, that match a search query.
    /// </summary>
    public Task<ICollection<GTStopPoint>> GetStopPoints(string searchQuery, int maxResults, CancellationToken ct = default);
    
    /// <summary>
    /// Returns a list of StopPoints, their lines and line modes, around a specific point.
    /// </summary>
    public Task<ICollection<GTStopPoint>> GetStopPoints(Point searchPoint, int searchRadius, int maxResults, CancellationToken ct = default);

    /// <summary>
    /// Returns a list of StopPoints that are children of a given StopPoint.
    /// </summary>
    public Task<ICollection<GTStopPoint>> GetAllChildrenOf(string stopPointId, CancellationToken ct = default);

    /// <summary>
    /// Returns the ids of all child stop points of a given stop point.
    /// </summary>
    public Task<ICollection<string>> GetChildIdsOf(string stopPointId, CancellationToken ct = default);

    /// <summary>
    /// Returns the ids of a stop points under a given HUB id
    /// </summary>
    public Task<ICollection<string>> GetIdsOfStopsAtHub(string hubId, CancellationToken ct = default);

    /// <summary>
    /// Returns a specified stop point from the database, if it exists
    /// </summary>
    public Task<GTStopPoint?> GetStopPoint(string id, CancellationToken ct = default);
    
    /// <summary>
    /// Performs an UPSERT on a StopPoint
    /// </summary>
    Task<GTStopPoint> Update(GTStopPoint stop, CancellationToken ct = default);

    /// <summary>
    /// Removes/deletes all StopPointInfo values for a specific stop from the database
    /// </summary>
    Task RemoveInfoValues(string stopPointId, CancellationToken ct = default);
    
    /// <summary>
    /// Inserts a collection of new GTStopPointInfoValue objects
    /// </summary>
    Task InsertInfoValues(IEnumerable<GTStopPointInfoValue> values, CancellationToken ct = default);

    /// <summary>
    /// Returns a list of all GTStopPointInfoValue results for a stop point
    /// </summary>
    public Task<ICollection<GTStopPointInfoValue>> GetInfoForStop(string id, CancellationToken ct = default);

    /// <summary>
    /// Returns the minimum information, i.e. id and name, for requested stops
    /// </summary>
    public Task<ICollection<GTStopPoint>> GetMinimumInfoFor(ICollection<string> ids, CancellationToken ct = default);

}