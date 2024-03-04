namespace GoTravel.API.Domain.Models.DTOs;

public enum CrowdsourceVoteStatus
{
    Upvoted,
    Downvoted,
    NoVote
}
public class CrowdsourceInfoDto
{
    public string CrowdsourceId { get; set; }
    public string? Text { get; set; }
    public bool IsDelayed { get; set; }
    public bool IsClosed { get; set; }
    
    public DateTime Started { get; set; }
    public DateTime ExpectedEnd { get; set; }
    public bool IsFlagged { get; set; }

    public UserDto SubmittedBy { get; set; }
    
    public CrowdsourceVoteStatus CurrentUserVoteStatus { get; set; }
    public double Score { get; set; }
}