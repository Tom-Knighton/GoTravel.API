namespace GoTravel.API.Domain.Models.DTOs;

public class UserDto
{
    public string UserName { get; set; }
    public string UserPictureUrl { get; set; }
    public ICollection<string> Roles { get; set; }
}