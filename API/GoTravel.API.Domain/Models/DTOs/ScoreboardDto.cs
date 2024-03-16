namespace GoTravel.API.Domain.Models.DTOs;

public class ScoreboardDto
{
    public string ScoreboardName { get; set; }
    public string? ScoreboardDescription { get; set; }
    public string? ScoreboardLogoUrl { get; set; }
    
    public ICollection<ScoreboardUserDto> ScoreboardUsers { get; set; }
}