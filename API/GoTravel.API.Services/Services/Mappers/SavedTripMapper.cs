using GoTravel.API.Domain.Models.Database;
using GoTravel.API.Domain.Models.DTOs;
using GoTravel.API.Domain.Services.Mappers;

namespace GoTravel.API.Services.Services.Mappers;

public class SavedTripMapper(IMapper<GTLine, LineDto> lineMap): IMapper<GTUserSavedJourney, UserSavedJourneyDto>
{
    public UserSavedJourneyDto Map(GTUserSavedJourney source)
    {
        var dto = new UserSavedJourneyDto
        {
            JourneyId = source.UUID,
            JourneyName = source.Name ?? "Journey",
            StartedAt = source.StartedAt,
            EndedAt = source.EndedAt,
            PointsReceived = source.Points,
            IsUnderReview = source.NeedsModeration,
            Note = source.Note,
            Coordinates = source.LineString.CoordinateSequence.ToCoordinateArray().Select(c => new List<double> { c.X, c.Y }).ToList(),
            Lines = source.Lines?.Select(l => lineMap.Map(l.Line)).ToList() ?? new()
        };

        return dto;
    }
}