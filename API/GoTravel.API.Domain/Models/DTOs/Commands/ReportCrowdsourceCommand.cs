using System.ComponentModel.DataAnnotations;

namespace GoTravel.API.Domain.Models.DTOs.Commands;

public class ReportCrowdsourceCommand
{
    [Required]
    [MaxLength(255)]
    public string ReportReason { get; set; }
}