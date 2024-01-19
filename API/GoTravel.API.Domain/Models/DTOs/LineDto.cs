namespace GoTravel.API.Domain.Models.DTOs;

/// <summary>
/// Represents a line operating under a line mode, i.e. the Central Line under the Tube line mode
/// </summary>
public class LineDto
{
    public string LineId { get; set; }
    public string LineName { get; set; }
    public string? LinePrimaryColour { get; set; }
}