namespace GoTravel.API.Domain.Models.Database;

public class GTUserSubtitle
{
    public int SubtitleId { get; set; }
    public string UserId { get; set; }
    public string Title { get; set; }
    public bool IsActive { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ExpiresAt { get; set; }
    
    public virtual GTUserDetails User { get; set; }
}