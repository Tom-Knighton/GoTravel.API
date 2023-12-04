using NetTopologySuite.Geometries;

namespace GoTravel.API.Domain.Models.Database;

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
    public int? BikesAvailable { get; set; }
    public int? EBikesAvailable { get; set; }

    public virtual ICollection<GLStopPointLine> StopPointLines { get; set; } = new List<GLStopPointLine>();
    public virtual ICollection<GLStopPoint> Children { get; set; } = new List<GLStopPoint>();
    public virtual GLStopPoint? Parent { get; set; }
}