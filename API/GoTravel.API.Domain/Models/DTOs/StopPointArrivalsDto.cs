using GoTravel.Standard.Models.Arrivals;

namespace GoTravel.API.Domain.Models.DTOs;

public class StopPointArrivalsDto
{
    public string StopPointId { get; set; }
    public ICollection<StopPointArrivalsMode> ModeArrivals { get; set; }
}

public class StopPointArrivalsMode
{
    public string ModeId { get; set; }
    public ICollection<LineArrivals> LineArrivals { get; set; }
}