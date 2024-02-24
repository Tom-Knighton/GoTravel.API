using GoTravel.API.Domain.Models.Database;
using GoTravel.API.Domain.Services.Mappers;
using GoTravel.Standard.Models.MessageModels;
using NetTopologySuite.Geometries;

namespace GoTravel.API.Services.Services.Mappers;

public class StopPointUpdateMapper: IMapper<StopPointUpdateDto, GTStopPoint>
{
    public GTStopPoint Map(StopPointUpdateDto source)
    {
        var isBus = source.Indicator is not null || source.Letter is not null;
        var stopPoint = new GTStopPoint
        {
            StopPointId = source.Id,
            StopPointName = source.Name ?? "",
            StopPointParentId = source.ParentId,
            StopPointType = isBus ? GTStopPointType.BusStopPoint : GTStopPointType.TrainStopPoint,
            BusStopIndicator = source.Indicator,
            BusStopLetter = source.Letter,
            StopPointCoordinate = new Point(source.Longitude ?? 0, source.Latitude ?? 0),
            BusStopSMSCode = source.SMS,
            StopPointHub = source.HubId,
            StopPointLines = source.Lines?.Select(x => new GLStopPointLine
            {
                StopPointId = source.Id,
                IsEnabled = false,
                LineId = x,
            }).ToList() ?? new List<GLStopPointLine>()
        };

        return stopPoint;
    }
}