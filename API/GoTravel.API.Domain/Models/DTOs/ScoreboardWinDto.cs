namespace GoTravel.API.Domain.Models.DTOs;

public enum ScoreboardWinRewardType
{
    None,
    PointMultiplier_1_5,
    PointMultiplier_2,
    StartingPoints_30,
    StartingPoints_10,
    Title_SuperTraveller,
    Title_PublicTransportExpert
}

public class ScoreboardWinDto
{
    public string WinId { get; set; }
    public string ScoreboardName { get; set; }
    public DateTime WonAt { get; set; }
    public int Position { get; set; }
    
    public ScoreboardWinRewardType RewardType { get; set; }
}