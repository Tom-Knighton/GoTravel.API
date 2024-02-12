namespace GoTravel.API.Domain.Models.Database;

public class GTUserDetails
{
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string UserProfilePicUrl { get; set; }
    public DateTime DateCreated { get; set; }
}