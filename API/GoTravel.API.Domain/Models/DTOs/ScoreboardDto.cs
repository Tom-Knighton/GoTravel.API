namespace GoTravel.API.Domain.Models.DTOs;

public class ScoreboardDto
{
    public string ScoreboardId { get; set; }
    public string ScoreboardName { get; set; }
    public string? ScoreboardDescription { get; set; }
    public string? ScoreboardLogoUrl { get; set; }
    
    public bool DoesReset { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    
    public ICollection<ScoreboardUserDto> ScoreboardUsers { get; set; }
}