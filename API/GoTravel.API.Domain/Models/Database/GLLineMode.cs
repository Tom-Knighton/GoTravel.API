namespace GoTravel.API.Domain.Models.Database;

public class GLLineMode
{
    public string LineModeName { get; set; }
    public bool IsEnabled { get; set; }
    
    public virtual ICollection<GLLine> Lines { get; set; }
}