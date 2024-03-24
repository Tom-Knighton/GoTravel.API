namespace GoTravel.API.Domain.Models.Database;

public class GTUserSavedJourneyLine
{
    public string SavedJourneyId { get; set; }
    public string LineId { get; set; }
    
    public virtual GTUserSavedJourney Journey { get; set; }
    public virtual GTLine Line { get; set; }
}