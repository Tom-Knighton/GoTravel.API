using NetTopologySuite.Geometries;

namespace GoTravel.API.Domain.Models.DTOs.Commands;

public class SaveUserTripCommand
{
    public DateTime StartedAt { get; set; }
    public DateTime EndedAt { get; set; }
    public string Name { get; set; }
    public double AverageSpeed { get; set; }
    
    public ICollection<ICollection<double>> Coordinates { get; set; }
    
    public ICollection<string> Lines { get; set; }
}