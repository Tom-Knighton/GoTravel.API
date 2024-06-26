using System.Collections;

namespace GoTravel.API.Domain.Models.Database;

public class GTLine
{
    public string LineId { get; set; }
    public string LineName { get; set; }
    public bool IsEnabled { get; set; }
    public string LineModeId { get; set; }
    public string? BrandingColour { get; set; }
    
    public virtual GLLineMode LineMode { get; set; }
    public virtual ICollection<GLStopPointLine> StopPointLines { get; set; }
    public virtual ICollection<GTLineRoute> Routes { get; set; }
}