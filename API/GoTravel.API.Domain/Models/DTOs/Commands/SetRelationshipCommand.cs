namespace GoTravel.API.Domain.Models.DTOs.Commands;

public class SetRelationshipCommand
{
    public string FollowingId { get; set; }
    public bool Follow { get; set; }
}