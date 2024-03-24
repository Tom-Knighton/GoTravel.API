using NetTopologySuite.Geometries;

namespace GoTravel.API.Domain.Models.Database;

public class GTUserSavedJourney
{
    public string UUID { get; set; }
    public string UserId { get; set; }

    public DateTime StartedAt { get; set; }
    public DateTime EndedAt { get; set; }
    public DateTime SubmittedAt { get; set; }
    
    public LineString LineString { get; set; }
    
    public bool NeedsModeration { get; set; }
    
    public int Points { get; set; }
    public string? Note { get; set; }
    
    public virtual ICollection<GTUserSavedJourneyLine> Lines { get; set; }
}