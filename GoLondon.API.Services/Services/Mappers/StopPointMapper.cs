using GoLondon.API.Domain.Models.Database;
using GoLondon.API.Domain.Models.DTOs;
using GoLondon.API.Domain.Services.Mappers;

namespace GoLondon.API.Services.Services.Mappers;

public class StopPointMapper: IMapper<GLStopPoint, StopPointBaseDto>
{
    public StopPointBaseDto Map(GLStopPoint src)
    {
        var dest = CreateBaseTypeDto(src);
        dest.StopPointId = src.StopPointId;
        dest.StopPointName = src.StopPointName;
        dest.StopPointParentId = src.StopPointParentId;
        dest.StopPointHub = src.StopPointHub;

        dest = src.StopPointType switch
        {
            GLStopPointType.TrainStopPoint => MapAsTrainStop(src, dest),
            GLStopPointType.BusStopPoint => MapAsBusStop(src, dest),
            GLStopPointType.BikeStopPoint => MapAsBikeStop(src, dest),
            _ => throw new ArgumentOutOfRangeException()
        };
        
        
        dest.Children = src.Children?.Select(Map).ToList();

        var lineLinks = src.StopPointLines;
        var lines = lineLinks.Select(sl => sl.Line);
        var grouped = lines
            .GroupBy(l => l.LineMode)
            .Select(group => new LineModeDto
            {
                LineModeName = group.Key.LineModeName,
                Lines = group.Select(v => new LineDto { LineName = v.LineName }).ToList()
            });

        dest.LineModes = grouped.ToList();

        return dest;
    }

    private StopPointBaseDto CreateBaseTypeDto(GLStopPoint src)
    {
        return src.StopPointType switch {
            GLStopPointType.TrainStopPoint => new TrainStopPointDto(),
            GLStopPointType.BusStopPoint => new BusStopPointDto(),
            GLStopPointType.BikeStopPoint => new BikeStopPointDto(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    
    private BusStopPointDto MapAsBusStop(GLStopPoint src, StopPointBaseDto dest)
    {
        var busStop = (BusStopPointDto)dest;

        busStop.BusStopIndicator = src.BusStopIndicator;
        busStop.BusStopLetter = src.BusStopLetter;
        busStop.BusStopSMSCode = src.BusStopSMSCode;
        
        return busStop;
    }

    private TrainStopPointDto MapAsTrainStop(GLStopPoint src, StopPointBaseDto dest)
    {
        var trainStop = (TrainStopPointDto)dest;

        return trainStop;
    }

    private BikeStopPointDto MapAsBikeStop(GLStopPoint src, StopPointBaseDto dest)
    {
        var bikeStop = (BikeStopPointDto)dest;

        bikeStop.BikesRemaining = src.BikesAvailable ?? 0;
        bikeStop.EBikesRemaining = src.EBikesAvailable ?? 0;

        return bikeStop;
    }
}