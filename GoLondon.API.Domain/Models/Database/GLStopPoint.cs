using NetTopologySuite.Geometries;

namespace GoLondon.API.Domain.Models.Database;

public enum GLStopPointType
{
    TrainStopPoint,
    BusStopPoint,
    BikeStopPoint
}

public class GLStopPoint
{
    public string StopPointId { get; set; }
    public GLStopPointType StopPointType { get; set; }
    public string StopPointName { get; set; }
    public Point StopPointCoordinate { get; set; }
    public string? StopPointHub { get; set; }
    public string? StopPointParentId { get; set; }
    public string? BusStopIndicator { get; set; }
    public string? BusStopLetter { get; set; }
    public string? BusStopSMSCode { get; set; }
    public string? BikesAvailable { get; set; }
    public string? EBikesAvailable { get; set; }
    
    public virtual ICollection<GLStopPointLine> StopPointLines { get; set; }
}