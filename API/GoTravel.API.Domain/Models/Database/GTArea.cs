using NetTopologySuite.Geometries;

namespace GoTravel.API.Domain.Models.Database;

public class GTArea
{
    public int AreaId { get; set; }
    
    public string AreaName { get; set; }
    
    public Polygon AreaCatchment { get; set; }
    
    public virtual ICollection<GLLineMode> LineModes { get; set; }
}