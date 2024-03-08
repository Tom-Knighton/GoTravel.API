using System.ComponentModel.DataAnnotations;

namespace GoTravel.API.Domain.Models.DTOs.Commands;

public class AddCrowdsourceCommand
{
    [MaxLength(128)]
    public string? FreeText { get; set; }
    public bool IsDelayed { get; set; }
    public bool IsClosed { get; set; }
    
    public DateTime StartsAt { get; set; }
    public DateTime ExpectedEnd { get; set; }
}