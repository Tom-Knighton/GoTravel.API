namespace GoTravel.API.Domain.Models.Database;

public class GTUserPointsAudit
{
    public string UserId { get; set; }
    public int PointsAtAdd { get; set; }
    public int PointsAdded { get; set; }
    public string Reason { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public virtual GTUserDetails User { get; set; }
}