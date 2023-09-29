namespace GoLondon.API.Domain.Models.DTOs;

/// <summary>
/// Represents a stop point for Santander bikes
/// </summary>
public class BikeStopPointDto: StopPointBaseDto
{
    /// <summary>
    /// The number of bikes available at this stop point
    /// </summary>
    public int BikesRemaining { get; set; }
    
    /// <summary>
    /// The number of e-bikes available at this stop point
    /// </summary>
    public int EBikesRemaining { get; set; }
}