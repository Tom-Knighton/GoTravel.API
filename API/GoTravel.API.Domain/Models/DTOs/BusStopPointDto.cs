namespace GoTravel.API.Domain.Models.DTOs;

/// <summary>
/// Represents a stop point for buses.
/// </summary>
public class BusStopPointDto: StopPointBaseDto
{
    /// <summary>
    /// Usually some kind of indicator if the bus stop has no 'letter', i.e. ->W for a westbound stop
    /// </summary>
    public string? BusStopIndicator { get; set; }
    
    /// <summary>
    /// The SMS code for the bus stop
    /// </summary>
    public string? BusStopSMSCode { get; set; }
    
    /// <summary>
    /// The 'letter' of the bus stop, i.e. A, B, C etc. May not be present
    /// </summary>
    public string? BusStopLetter { get; set; }
}