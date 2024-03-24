using GoTravel.API.Domain.Data;
using GoTravel.API.Domain.Extensions;
using GoTravel.API.Domain.Models.Database;
using GoTravel.API.Domain.Models.DTOs.Commands;
using GoTravel.API.Domain.Services;
using GoTravel.API.Domain.Services.Repositories;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace GoTravel.API.Services.Services;

public class TripService: ITripService
{
    private readonly GoTravelContext _context;
    private readonly ILineModeService _lineModeService;
    private readonly ITripRepository _repo;
    private const double BufferDistance = 0.001;
    private const double IntersectThresholdLength = 0.006;
    
    private const double PointsPerKm = 0.5;
    private const double CoverageThreshold = 50;
    private const double KmPerDegree = 111;

    public TripService(GoTravelContext context, ILineModeService lineMode, ITripRepository repo)
    {
        _context = context;
        _lineModeService = lineMode;
        _repo = repo;
    }

    public async Task SaveUserTrip(SaveUserTripCommand command, string userId, CancellationToken ct = default)
    {
        var line = new LineString(command.Coordinates.Select(c => new Coordinate(c.ElementAt(0), c.ElementAt(1))).ToArray());
        var buffer = line.Buffer(BufferDistance);

        var intersectedRoutes = await _context.LineRoutes
            .Where(r => r.Route.Intersects(buffer))
            .Select(r => new
            {
                LineRoute = r,
                r.Route.Intersection(buffer).Length
            })
            .Where(r => r.Length >= IntersectThresholdLength)
            .OrderByDescending(r => r.Length)
            .ToListAsync(ct);
        
        var unionOfIntersectedAreas = new GeometryCollection(intersectedRoutes.Select(g => g.LineRoute.Route.Buffer(BufferDistance).Intersection(buffer)).ToArray())
            .Union();
        var nonIntersectedAreas = buffer.Difference(unionOfIntersectedAreas);

        
        var nonIntersectedArea = nonIntersectedAreas.Area;
        var percentageUncovered = (nonIntersectedArea / buffer.Area) * 100;

        var journey = new GTUserSavedJourney
        {
            UUID = Guid.NewGuid().ToString("N"),
            UserId = userId,
            LineString = line,
            StartedAt = command.StartedAt,
            EndedAt = command.EndedAt,
            NeedsModeration = false,
            Lines = command.Lines.Select(l => new GTUserSavedJourneyLine
            {
                LineId = l
            }).ToList(),
            Points = GetPointsForTrip(line.Length, (command.EndedAt - command.StartedAt).Minutes, 100 - percentageUncovered)
        };

        if (percentageUncovered >= CoverageThreshold || !command.Lines.Most(l => intersectedRoutes.Any(r => r.LineRoute.LineId == l)))
        {
            journey.NeedsModeration = true;
        }

        journey = await _repo.SaveJourney(journey, ct);
    }

    private static int GetPointsForTrip(double distance, double time, double percentageCovered)
    {
        // Assign points in the following:
        // - (percentage covered by routes / 2) * km distance * points per km (0.5) rounded to 0dp +
        // - time bonus: 3 points for <= 10 mins, 5 points for <= 30m, 10 points for <= 1hr, beyond that: 10 + (time - 1hr) * 1.05

        var km = distance * KmPerDegree;
        
        // Points for covered distance
        var coveredPoints = (int)((percentageCovered / 2) * km * PointsPerKm);

        var timeBonus = time switch
        {
            > 60 => 10 + (int)((time - 60) * 1.05),
            > 30 and <= 60 => 10,
            > 10 and <= 30 => 5,
            > 0 and <= 10 => 3,
            _ => 0
        };

        return coveredPoints + timeBonus;
    }
}