using GoTravel.Standard.Models;

namespace GoTravel.API.Domain.Models.Database;

public class GTStopPointInfoValue
{
    public string StopPointId { get; set; }
    public StopPointInfoKey KeyId { get; set; }
    public string Value { get; set; }
    
    public virtual GLStopPoint StopPoint { get; set; }
    public virtual GTStopPointInfoKey Key { get; set; }
}