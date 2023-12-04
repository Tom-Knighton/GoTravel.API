using GoTravel.API.Domain.Models.Database;
using NetTopologySuite.Geometries;

namespace GoTravel.API.Domain.Services.Repositories;

public interface IStopPointRepository
{
    /// <summary>
    /// Returns a list of StopPoints, their lines and line modes, that match a search query.
    /// </summary>
    public Task<ICollection<GLStopPoint>> GetStopPoints(string searchQuery, int maxResults, CancellationToken ct = default);
    
    /// <summary>
    /// Returns a list of StopPoints, their lines and line modes, around a specific point.
    /// </summary>
    public Task<ICollection<GLStopPoint>> GetStopPoints(Point searchPoint, int searchRadius, int maxResults, CancellationToken ct = default);

    /// <summary>
    /// Returns a list of StopPoints that are children of a given StopPoint.
    /// </summary>
    public Task<ICollection<GLStopPoint>> GetAllChildrenOf(string stopPointId, CancellationToken ct = default);
}