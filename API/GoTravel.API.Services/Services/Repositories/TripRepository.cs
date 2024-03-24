using GoTravel.API.Domain.Data;
using GoTravel.API.Domain.Models.Database;
using GoTravel.API.Domain.Services.Repositories;

namespace GoTravel.API.Services.Services.Repositories;

public class TripRepository: ITripRepository
{
    private GoTravelContext _context;

    public TripRepository(GoTravelContext context)
    {
        _context = context;
    }
    
    public Task<GTUserSavedJourney> GetJourneysForUser(string userId, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task<GTUserSavedJourney> SaveJourney(GTUserSavedJourney journey, CancellationToken ct = default)
    {
        if (_context.UserSavedJourneys.Any(j => j.UUID == journey.UUID))
        {
            _context.UserSavedJourneys.Update(journey);
        }
        else
        {
            _context.UserSavedJourneys.Add(journey);
        }

        await _context.SaveChangesAsync(ct);
        return journey;
    }

    public Task<GTUserSavedJourney?> GetJourney(string journeyId, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}