using NetTopologySuite.Geometries;

namespace GoTravel.API.Domain.Models.Database;

public class GTLineRoute
{
    public string LineId { get; set; }
    public string Direction { get; set; }
    public string Name { get; set; }
    public string ServiceType { get; set; }
    
    public MultiLineString Route { get; set; }
    
    public virtual GTLine Line { get; set; }
}