namespace GoLondon.API.Domain.Models.DTOs;

/// <summary>
/// Represents a mode of transport
/// </summary>
public class LineModeDto
{
    /// <summary>
    /// The id and displayable name of the line i.e. Tube, Elizabeth Line etc.
    /// </summary>
    public string LineModeName { get; set; }
    
    /// <summary>
    /// The lines under this line mode
    /// </summary>
    public ICollection<LineDto> Lines { get; set; }
}