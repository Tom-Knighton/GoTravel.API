namespace GoTravel.API.Domain.Models.DTOs.Commands;

public class ApproveRejectFollowCommand
{
    public string UserId { get; set; }
    public bool Approve { get; set; }
}