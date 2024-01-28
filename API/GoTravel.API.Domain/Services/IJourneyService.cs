using GoTravel.API.Domain.Models.DTOs;
using GoTravel.Standard.Models.Journeys;

namespace GoTravel.API.Domain.Services;

public interface IJourneyService
{
    Task<JourneyOptionsResultDto> GetJourneys(JourneyRequest request, CancellationToken ct = default);
}