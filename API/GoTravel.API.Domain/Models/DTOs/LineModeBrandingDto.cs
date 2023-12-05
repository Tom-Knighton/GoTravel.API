namespace GoTravel.API.Domain.Models.DTOs;

/// <summary>
/// Branding information for a line mode
/// </summary>
public class LineModeBrandingDto
{
    /// <summary>
    /// The url to the logo for the line mode
    /// </summary>
    public string LineModeLogoUrl { get; set; }
    
    /// <summary>
    /// The hex colour code for the line mode's 'background'
    /// </summary>
    public string LineModeBackgroundColour { get; set; }
    
    /// <summary>
    /// The hex colour code for the line mode's branding colour, i.e. purple for Elizabeth Line
    /// </summary>
    public string LineModePrimaryColour { get; set; }
    
    /// <summary>
    /// The hex colour code for the line mode's secondary colour, i.e. red for tube
    /// </summary>
    public string? LineModeSecondaryColour { get; set; }
}