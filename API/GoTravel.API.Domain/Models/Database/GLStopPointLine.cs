namespace GoTravel.API.Domain.Models.Database;

public class GLStopPointLine
{
    public string StopPointId { get; set; }
    public string LineId { get; set; }
    public bool IsEnabled { get; set; }

    public virtual GTStopPoint StopPoint { get; set; }
    public virtual GTLine Line { get; set; }
}