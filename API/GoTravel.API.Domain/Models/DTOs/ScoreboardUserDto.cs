namespace GoTravel.API.Domain.Models.DTOs;

public class ScoreboardUserDto
{
    public int Rank { get; set; }
    public int Points { get; set; }
    public UserDto User { get; set; }
}