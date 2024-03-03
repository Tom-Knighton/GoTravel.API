namespace GoTravel.API.Domain.Models.Database;

public class GTUserDetails
{
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string UserProfilePicUrl { get; set; }
    public DateTime DateCreated { get; set; }
    public GTUserFollowerAcceptLevel FollowerAcceptType { get; set; }
    public int UserPoints { get; set; }
    
    public virtual ICollection<GTUserFollowings> FollowingUsers { get; set; }
    public virtual ICollection<GTUserFollowings> Followers { get; set; }
    
}

public enum GTUserFollowerAcceptLevel
{
    AcceptAll,
    RequiresApproval,
    NoFollowers
}