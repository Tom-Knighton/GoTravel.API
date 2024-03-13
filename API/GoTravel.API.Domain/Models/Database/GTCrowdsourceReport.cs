namespace GoTravel.API.Domain.Models.Database;

public class GTCrowdsourceReport
{
    public string UUID { get; set; }
    public string CrowdsourceId { get; set; }
    public string ReporterId { get; set; }
    public DateTime ReportedAt { get; set; }
    public string ReportText { get; set; }
    public bool Handled { get; set; }
    
    public virtual GTUserDetails Reporter { get; set; }
}