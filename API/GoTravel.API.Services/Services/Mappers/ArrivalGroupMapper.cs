using GoTravel.API.Domain.Services.Mappers;
using GoTravel.Standard.Models.Arrivals;

namespace GoTravel.API.Services.Services.Mappers;

public class ArrivalGroupMapper: IMapper<IEnumerable<ArrivalDeparture>, ICollection<LineArrivals>>
{
    public ICollection<LineArrivals> Map(IEnumerable<ArrivalDeparture> source)
    {
        var lines = source
            .Select(s => s.Line)
            .Distinct();

        var lineGroups = new List<LineArrivals>();

        foreach (var line in lines)
        {
            var arrivalsForLine = source.Where(s => s.Line == line).ToList();

            var platforms = arrivalsForLine.Select(a => a.Platform).Distinct();
            var platformGroups = platforms.Select(p =>
            {
                var platformGroup = new PlatformArrivals();
                var arrivals = arrivalsForLine.Where(a => a.Platform == p);
                var direction = arrivals.Select(x => x.Direction).Distinct()
                    .FirstOrDefault(d => !string.IsNullOrWhiteSpace(d));

                platformGroup.PlatformName = p;
                platformGroup.Direction = direction;
                platformGroup.ArrivalDepartures = arrivals.ToList();
                
                return platformGroup;
            });

            var lineGroup = new LineArrivals
            {
                Line = line,
                LineMode = arrivalsForLine.FirstOrDefault()?.LineMode ?? "",
                Platforms = platformGroups.ToList()
            };
            
            lineGroups.Add(lineGroup);
        }

        return lineGroups;
    }
}