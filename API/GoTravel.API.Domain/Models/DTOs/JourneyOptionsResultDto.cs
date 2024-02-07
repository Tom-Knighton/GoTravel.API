namespace GoTravel.API.Domain.Models.DTOs;

public class JourneyOptionsResultDto
{
    /// <summary>
    /// The possible journey options returned
    /// </summary>
    public ICollection<JourneyDto> JourneyOptions { get; set; } = new List<JourneyDto>();
}