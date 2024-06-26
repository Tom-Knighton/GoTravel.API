namespace GoTravel.API.Domain.Models.Database;

public class GLLineMode
{
    public string LineModeId { get; set; }
    public string LineModeName { get; set; }
    public bool IsEnabled { get; set; }
    
    public string LogoUrl { get; set; }
    public string BrandingColour { get; set; }
    public string PrimaryColour { get; set; }
    public string? SecondaryColour { get; set; }
    
    public int? AreaId { get; set; }

    public virtual ICollection<GTLine> Lines { get; set; } = new List<GTLine>();
    public virtual GTArea? PrimaryArea { get; set; }
    public virtual ICollection<GLFlag> Flags { get; set; } = new List<GLFlag>();
}