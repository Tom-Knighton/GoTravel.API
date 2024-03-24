namespace GoTravel.API.Domain.Models.DTOs;

public class UserSavedJourneyDto
{
    public string JourneyId { get; set; }
    public string JourneyName { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime EndedAt { get; set; }
    public int PointsReceived { get; set; }
    public int IsUnderReview { get; set; }
    public ICollection<ICollection<double>> Coordinates { get; set; }
}