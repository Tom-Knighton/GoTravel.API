namespace GoTravel.API.Domain.Models.Database;

public class GTScoreboardUser
{
    public string ScoreboardUUID { get; set; }
    public string UserId { get; set; }
    public int Points { get; set; }
    
    public DateTime JoinedAt { get; set; }
    
    public virtual GTScoreboard Scoreboard { get; set; }
    public virtual GTUserDetails User { get; set; }
}