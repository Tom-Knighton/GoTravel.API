namespace GoTravel.API.Domain.Models.DTOs;

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
    
    /// <summary>
    /// The line may have a primary area, i.e. London, Manchester etc. Will default to 'UK'
    /// </summary>
    public string PrimaryAreaName { get; set; }
    
    /// <summary>
    /// The branding information for the line mode
    /// </summary>
    public LineModeBrandingDto Branding { get; set; }
}