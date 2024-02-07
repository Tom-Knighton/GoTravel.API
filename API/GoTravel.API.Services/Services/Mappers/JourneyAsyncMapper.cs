using GoTravel.API.Domain.Models.DTOs;
using GoTravel.API.Domain.Services;
using GoTravel.API.Domain.Services.Mappers;
using GoTravel.Standard.Models.Journeys;
using NetTopologySuite.Geometries;

namespace GoTravel.API.Services.Services.Mappers;

public class JourneyAsyncMapper: IAsyncMapper<Journey, JourneyDto>
{
    private readonly ILineModeService _lineModes;
    private readonly IStopPointService _stopPoints;
    private const string WalkModeId = "walking";
    private const string CycleModeId = "cycle";

    public JourneyAsyncMapper(ILineModeService lm, IStopPointService sp)
    {
        _lineModes = lm;
        _stopPoints = sp;
    }
    
    
    public async Task<JourneyDto> MapAsync(Journey source)
    {
        var dto = new JourneyDto
        {
            BeginJourneyAt = source.BeginJourneyAt,
            EndJourneyAt = source.EndJourneyAt,
            TotalDuration = source.TotalDuration
        };

        var legs = new List<JourneyLegDto>();
        foreach (var leg in source.JourneyLegs)
        {
            var lineMode = leg.LegDetails.ModeId switch
            {
                WalkModeId => new LineModeDto { LineModeId = "walking", LineModeName = "Walk" },
                CycleModeId => new LineModeDto { LineModeId = "cycle", LineModeName = "Cycle" },
                _ => (await _lineModes.ListFromLineIdsAsync(leg.LegDetails.LineIds)).FirstOrDefault()
            };

            var stops = await _stopPoints.GetBasicLegStopPointDtos(leg.LegDetails.StopIds);
            var details = new JourneyLegDetailsDto
            {
                Summary = leg.LegDetails.Summary,
                DetailedSummary = leg.LegDetails.DetailedSummary,
                LegSteps = leg.LegDetails.LegSteps.Select(s => new JourneyLegStepDto
                {
                    Summary = s.Summary,
                    Direction = s.Direction,
                    Latitude = s.Latitude,
                    Longitude = s.Longitude,
                    DistanceOfStep = s.DistanceOfStep
                }).ToList(),
                LineMode = lineMode,
                LineString = new LineString(leg.LegDetails.LineString.Select(p => new Coordinate(p.ElementAt(1), p.ElementAt(0))).ToArray()),
                StopPoints = stops
            };
            
            
            var legDto = new JourneyLegDto
            {
                BeginLegAt = leg.BeginLegAt,
                EndLegAt = leg.EndLegAt,
                LegDuration = leg.LegDuration,
                StartAtName = leg.StartAtStopName,
                EndAtName = leg.EndAtStopName,
                StartAtStopId = leg.StartAtStopId,
                EndAtStopId = leg.EndAtStopId,
                LegDetails = details
            };
            legs.Add(legDto);
        }

        dto.JourneyLegs = legs;

        return dto;
    }
}