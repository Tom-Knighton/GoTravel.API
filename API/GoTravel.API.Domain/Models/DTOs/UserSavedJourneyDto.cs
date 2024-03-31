namespace GoTravel.API.Domain.Models.DTOs;

public class UserSavedJourneyDto
{
    public string JourneyId { get; set; }
    public string JourneyName { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime EndedAt { get; set; }
    public int PointsReceived { get; set; }
    public bool IsUnderReview { get; set; }
    public string? Note { get; set; }
    public IEnumerable<IEnumerable<double>> Coordinates { get; set; }
    
    public ICollection<LineDto> Lines { get; set; }
}