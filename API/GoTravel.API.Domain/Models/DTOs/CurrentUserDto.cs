namespace GoTravel.API.Domain.Models.DTOs;

public class CurrentUserDto
{
    public string UserId { get; set; }
    public string UserEmail { get; set; }
    public string UserName { get; set; }
    public string UserPictureUrl { get; set; }
    public DateTime DateCreated { get; set; }
    public ICollection<string> Roles { get; set; }
}