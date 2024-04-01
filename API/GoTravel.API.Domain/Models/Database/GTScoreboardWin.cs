using GoTravel.API.Domain.Models.DTOs;

namespace GoTravel.API.Domain.Models.Database;

public class GTScoreboardWin
{
    public string UUID { get; set; }
    public string ScoreboardId { get; set; }
    public string UserId { get; set; }
    public int ScoreboardPosition { get; set; }
    public DateTime WonAt { get; set; }
    public bool HasSeen { get; set; }
    
    public ScoreboardWinRewardType RewardType { get; set; }
    
    public virtual GTScoreboard Scoreboard { get; set; }
    public virtual GTUserDetails User { get; set; }
}