namespace GoTravel.API.Domain.Models.DTOs;

public enum FollowingType
{
    Requested,
    Following,
    Blocked
}

public class UserFollowingDto
{
    public FollowingType FollowingType { get; set; }
    public UserDto User { get; set; }
}