using GoTravel.API.Domain.Data;
using GoTravel.API.Domain.Models.Database;
using GoTravel.API.Domain.Services.Repositories;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace GoTravel.API.Services.Services.Repositories;

public class TripRepository: ITripRepository
{
    private GoTravelContext _context;

    public TripRepository(GoTravelContext context)
    {
        _context = context;
    }

    public async Task<ICollection<GTUserSavedJourney>> GetJourneysForUser(string userId, int results, int startFrom, CancellationToken ct = default)
    {
        var trips = await _context.UserSavedJourneys
            .Include(j => j.Lines)
            .ThenInclude(l => l.Line)
            .Where(j => j.UserId == userId)
            .OrderByDescending(j => j.SubmittedAt)
            .Skip(startFrom - 1)
            .Take(results)
            .ToListAsync(ct);

        return trips;
    }

    public async Task<ICollection<Intersection>> GetIntersections(Geometry buffer, double threshold, CancellationToken ct = default)
    {
        var intersectedRoutes = await _context.LineRoutes
            .Where(r => r.Route.Intersects(buffer))
            .Select(r => new
            {
                LineRoute = r,
                r.Route.Intersection(buffer).Length
            })
            .Where(r => r.Length >= threshold)
            .OrderByDescending(r => r.Length)
            .ToListAsync(ct);

        return intersectedRoutes.Select(r => new Intersection(r.LineRoute, r.Length)).ToList();
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