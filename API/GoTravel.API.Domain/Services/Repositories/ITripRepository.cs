using GoTravel.API.Domain.Models.Database;

namespace GoTravel.API.Domain.Services.Repositories;

public interface ITripRepository
{
    public Task<GTUserSavedJourney> GetJourneysForUser(string userId, CancellationToken ct = default);
    public Task<GTUserSavedJourney> SaveJourney(GTUserSavedJourney journey, CancellationToken ct = default);
    public Task<GTUserSavedJourney?> GetJourney(string journeyId, CancellationToken ct = default);
}