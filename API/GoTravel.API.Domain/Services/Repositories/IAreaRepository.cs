using GoTravel.API.Domain.Models.Database;
using NetTopologySuite.Geometries;

namespace GoTravel.API.Domain.Services.Repositories;

public interface IAreaRepository
{
    Task<GTArea?> GetAreaFromPoint(Point point, CancellationToken ct = default);
}