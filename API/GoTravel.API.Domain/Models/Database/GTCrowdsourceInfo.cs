namespace GoTravel.API.Domain.Models.Database;

public class GTCrowdsourceInfo
{
    public string UUID { get; set; }
    public string EntityId { get; set; }
    public string? FreeText { get; set; }
    public bool IsDelayed { get; set; }
    public bool IsClosed { get; set; }
    public DateTime SubmittedAt { get; set; }
    public DateTime ExpectedEnd { get; set; }
    public string SubmittedById { get; set; }
    public bool NeedsReview { get; set; }
    
    public virtual GTUserDetails SubmittedBy { get; set; }
    public virtual ICollection<GTCrowdsourceVotes> Votes { get; set; }
}