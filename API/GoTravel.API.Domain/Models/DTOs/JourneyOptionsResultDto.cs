using GoTravel.Standard.Models.Journeys;

namespace GoTravel.API.Domain.Models.DTOs;

public class JourneyOptionsResultDto
{
    /// <summary>
    /// The possible journey options returned
    /// </summary>
    public ICollection<Journey> JourneyOptions { get; set; } = new List<Journey>();

    /// <summary>
    /// All the line modes detailed in the journey options - will only have lines attached that are present in journey options
    /// </summary>
    public ICollection<LineModeDto> LineModes { get; set; } = new List<LineModeDto>();
}