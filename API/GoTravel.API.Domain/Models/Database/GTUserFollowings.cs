using GoTravel.API.Domain.Models.DTOs;

namespace GoTravel.API.Domain.Models.Database;

public class GTUserFollowings
{
    public string RequesterId { get; set; }
    public string FollowsId { get; set; }
    public bool IsAccepted { get; set; }
    public DateTime Created { get; set; }
    public bool DoesFollow { get; set; }
    
    public virtual GTUserDetails Requester { get; set; }
    public virtual GTUserDetails Follows { get; set; }
}