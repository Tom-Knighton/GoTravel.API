using GoTravel.API.Domain.Models.Database;
using NetTopologySuite.Geometries;

namespace GoTravel.API.Domain.Services.Repositories;

public interface ITripRepository
{
    public Task<ICollection<GTUserSavedJourney>> GetJourneysForUser(string userId, int results, int startFrom, CancellationToken ct = default);

    public Task<ICollection<Intersection>> GetIntersections(Geometry buffer, double threshold, CancellationToken ct = default);
    public Task<GTUserSavedJourney> SaveJourney(GTUserSavedJourney journey, CancellationToken ct = default);
    public Task<GTUserSavedJourney?> GetJourney(string journeyId, CancellationToken ct = default);
}

public record Intersection(GTLineRoute route, double length);