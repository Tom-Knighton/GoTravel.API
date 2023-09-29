namespace GoLondon.API.Domain.Models.DTOs;

/// <summary>
/// Represents a kind of stop point
/// </summary>
public class StopPointBaseDto
{
    /// <summary>
    /// The ID (Naptan) of the stop point.
    /// </summary>
    public string StopPointId { get; set; }
    
    /// <summary>
    /// The friendly name of the stop point
    /// </summary>
    public string StopPointName { get; set; }
    
    /// <summary>
    /// The StopPointId of the HUB this stop point belongs to (if it does at all).
    /// </summary>
    public string? StopPointHub { get; set; }
    
    /// <summary>
    /// The StopPointId of the parent stop point (if it exists).
    /// </summary>
    public string? StopPointParentId { get; set; }
    
    /// <summary>
    /// The child stop points of this stop
    /// </summary>
    public ICollection<StopPointBaseDto> Children { get; set; }
    
    /// <summary>
    /// The line modes served by this stop point, each line mode contains the lines served here
    /// </summary>
    public ICollection<LineModeDto> LineModes { get; set; }
}