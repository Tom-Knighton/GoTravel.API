namespace GoTravel.API.Domain.Models.DTOs.Commands;

public class AddCrowdsourceCommand
{
    public string EntityId { get; set; }
    public string? FreeText { get; set; }
    public bool IsDelayed { get; set; }
    public bool IsClosed { get; set; }
}