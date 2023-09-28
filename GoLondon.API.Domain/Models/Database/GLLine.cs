namespace GoLondon.API.Domain.Models.Database;

public class GLLine
{
    public string LineId { get; set; }
    public string LineName { get; set; }
    public bool IsEnabled { get; set; }
    public string LineModeId { get; set; }
    
    public virtual GLLineMode LineMode { get; set; }
    public virtual ICollection<GLStopPointLine> StopPointLines { get; set; }
}