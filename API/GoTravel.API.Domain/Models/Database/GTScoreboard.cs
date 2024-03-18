namespace GoTravel.API.Domain.Models.Database;

public enum GTScoreboadJoinType
{
    AllEnrolled,
    ManualEnroll,
    RequestEnroll,
    Private,
    Disabled
}

public class GTScoreboard
{
    public string UUID { get; set; }
    public string ScoreboardName { get; set; }
    public string? ScoreboardIconUrl { get; set; }
    public string ScoreboardDescription { get; set; }
    public DateTime ActiveFrom { get; set; }
    public DateTime? EndsAt { get; set; }
    public GTScoreboadJoinType JoinType { get; set; }
    
    public virtual ICollection<GTScoreboardUser> Users { get; set; }
}