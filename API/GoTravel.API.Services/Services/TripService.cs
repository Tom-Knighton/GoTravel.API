using GoTravel.API.Domain.Extensions;
using GoTravel.API.Domain.Models.Database;
using GoTravel.API.Domain.Models.DTOs;
using GoTravel.API.Domain.Models.DTOs.Commands;
using GoTravel.API.Domain.Models.Lib;
using GoTravel.API.Domain.Services;
using GoTravel.API.Domain.Services.Mappers;
using GoTravel.API.Domain.Services.Repositories;
using MassTransit;
using NetTopologySuite.Geometries;

namespace GoTravel.API.Services.Services;

public class TripService: ITripService
{
    private readonly IMapper<GTUserSavedJourney, UserSavedJourneyDto> _map;
    private readonly ITripRepository _repo;
    private readonly IPublishEndpoint _publisher;
    private readonly TimeProvider _time;
    private const double BufferDistance = 0.001;
    private const double IntersectThresholdLength = 0.006;
    
    private const double PointsPerKm = 0.5;
    private const double CoverageThreshold = 50;
    private const double KmPerDegree = 111;

    public TripService(IMapper<GTUserSavedJourney, UserSavedJourneyDto> mapper, ITripRepository repo,  IPublishEndpoint publisher, TimeProvider time)
    {
        _map = mapper;
        _repo = repo;
        _publisher = publisher;
        _time = time;
    }

    public async Task<UserSavedJourneyDto?> SaveUserTrip(SaveUserTripCommand command, string userId, CancellationToken ct = default)
    {
        var line = new LineString(command.Coordinates.Select(c => new Coordinate(c.ElementAt(0), c.ElementAt(1))).ToArray());
        var buffer = line.Buffer(BufferDistance);

        var intersectedRoutes = await _repo.GetIntersections(buffer, IntersectThresholdLength, ct);
        
        var unionOfIntersectedAreas = new GeometryCollection(intersectedRoutes.Select(g => g.route.Route.Buffer(BufferDistance).Intersection(buffer)).ToArray())
            .Union();
        var nonIntersectedAreas = buffer.Difference(unionOfIntersectedAreas);

        
        var nonIntersectedArea = nonIntersectedAreas.Area;
        var percentageUncovered = (nonIntersectedArea / buffer.Area) * 100;

        var journey = new GTUserSavedJourney
        {
            UUID = Guid.NewGuid().ToString("N"),
            UserId = userId,
            LineString = line,
            Name = command.Name,
            StartedAt = command.StartedAt.ToUniversalTime(),
            EndedAt = command.EndedAt.ToUniversalTime(),
            SubmittedAt = _time.GetUtcNow().UtcDateTime,
            NeedsModeration = false,
            Lines = command.Lines.Select(l => new GTUserSavedJourneyLine
            {
                LineId = l
            }).ToList(),
            Points = GetPointsForTrip(line.Length, (command.EndedAt - command.StartedAt).Minutes, 100 - percentageUncovered, command.Lines.Count != 0)
        };

        if (percentageUncovered >= CoverageThreshold || (command.Lines.Count != 0 && !command.Lines.Most(l => intersectedRoutes.Any(r => r.route.LineId == l))))
        {
            journey.NeedsModeration = true;
        }

        journey = await _repo.SaveJourney(journey, ct);

        if (!journey.NeedsModeration)
        {
            await _publisher.Publish(new AddPointsMessage { UserId = userId, Message = $"User saved trip {journey.UUID} for {journey.Points} points.", Points = journey.Points }, ct);
        }

        return _map.Map(journey);
    }

    public async Task<ICollection<UserSavedJourneyDto>> GetTripsForUser(string userId, int results, int startFrom, CancellationToken ct = default)
    {
        var trips = await _repo.GetJourneysForUser(userId, results, startFrom, ct);
        var dtos = trips.Select(t => _map.Map(t));
        
        return dtos.ToList();
    }

    private static int GetPointsForTrip(double distance, double time, double percentageCovered, bool postedLines)
    {
        // Assign points in the following:
        // - (percentage covered by routes / 2) * km distance * points per km (0.5) rounded to 0dp +
        // - time bonus: 3 points for <= 10 mins, 5 points for <= 30m, 10 points for <= 1hr, beyond that: 10 + (time - 1hr) * 1.05

        var km = distance * KmPerDegree;
        
        // Points for covered distance
        var coveredPoints = (int)((percentageCovered / (postedLines ? 2 : 2.5)) * km * PointsPerKm);

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