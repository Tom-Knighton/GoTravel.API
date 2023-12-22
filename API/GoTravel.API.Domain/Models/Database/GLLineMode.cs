namespace GoTravel.API.Domain.Models.Database;

public class GLLineMode
{
    public string LineModeName { get; set; }
    public bool IsEnabled { get; set; }
    
    public string LogoUrl { get; set; }
    public string BrandingColour { get; set; }
    public string PrimaryColour { get; set; }
    public string? SecondaryColour { get; set; }
    
    public int? AreaId { get; set; }
    
    public virtual ICollection<GLLine> Lines { get; set; }
    public virtual GTArea? PrimaryArea { get; set; }
    public virtual ICollection<GLFlag> Flags { get; set; }
}