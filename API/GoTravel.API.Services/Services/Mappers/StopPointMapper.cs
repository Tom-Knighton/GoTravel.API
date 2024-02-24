using GoTravel.API.Domain.Models.Database;
using GoTravel.API.Domain.Models.DTOs;
using GoTravel.API.Domain.Services.Mappers;

namespace GoTravel.API.Services.Services.Mappers;

public class StopPointMapper: IMapper<GTStopPoint, StopPointBaseDto>
{

    private IMapper<GLLineMode, LineModeDto> _lineModeMapper;

    public StopPointMapper(IMapper<GLLineMode, LineModeDto> lineModeMapper)
    {
        _lineModeMapper = lineModeMapper;
    }
    
    public StopPointBaseDto Map(GTStopPoint src)
    {
        var dest = CreateBaseTypeDto(src);
        dest.StopPointId = src.StopPointId;
        dest.StopPointName = src.StopPointName;
        dest.StopPointParentId = src.StopPointParentId;
        dest.StopPointHub = src.StopPointHub;
        dest.StopPointCoordinate = src.StopPointCoordinate;
        dest.StopPointType = MapType(src.StopPointType);

        dest = src.StopPointType switch
        {
            GTStopPointType.TrainStopPoint => MapAsTrainStop(src, dest),
            GTStopPointType.BusStopPoint => MapAsBusStop(src, dest),
            GTStopPointType.BikeStopPoint => MapAsBikeStop(src, dest),
            _ => throw new ArgumentOutOfRangeException()
        };
        
        
        dest.Children = src.Children?.Select(Map).ToList();

        var lineLinks = src.StopPointLines;
        var lines = lineLinks.Select(sl => sl.Line).ToList();
        var modes = lines
            .Select(x => x.LineMode)
            .DistinctBy(x => x.LineModeName);
        
        var newLineModes = new List<LineModeDto>();
        foreach (var mode in modes)
        {
            mode.Lines = lines.Where(l => l.LineModeId == mode.LineModeId).ToList();
            var newMode = _lineModeMapper.Map(mode);
            newLineModes.Add(newMode);
        }

        dest.LineModes = newLineModes;

        return dest;
    }

    private StopPointBaseDto CreateBaseTypeDto(GTStopPoint src)
    {
        return src.StopPointType switch {
            GTStopPointType.TrainStopPoint => new TrainStopPointDto(),
            GTStopPointType.BusStopPoint => new BusStopPointDto(),
            GTStopPointType.BikeStopPoint => new BikeStopPointDto(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    
    private BusStopPointDto MapAsBusStop(GTStopPoint src, StopPointBaseDto dest)
    {
        var busStop = (BusStopPointDto)dest;

        busStop.BusStopIndicator = src.BusStopIndicator;
        busStop.BusStopLetter = src.BusStopLetter;
        busStop.BusStopSMSCode = src.BusStopSMSCode;
        
        return busStop;
    }

    private TrainStopPointDto MapAsTrainStop(GTStopPoint src, StopPointBaseDto dest)
    {
        var trainStop = (TrainStopPointDto)dest;

        return trainStop;
    }

    private BikeStopPointDto MapAsBikeStop(GTStopPoint src, StopPointBaseDto dest)
    {
        var bikeStop = (BikeStopPointDto)dest;

        bikeStop.BikesRemaining = src.BikesAvailable ?? 0;
        bikeStop.EBikesRemaining = src.EBikesAvailable ?? 0;

        return bikeStop;
    }

    private StopPointType MapType(GTStopPointType gtType)
    {
        return gtType switch
        {
            GTStopPointType.TrainStopPoint => StopPointType.Train,
            GTStopPointType.BusStopPoint => StopPointType.Bus,
            GTStopPointType.BikeStopPoint => StopPointType.Bike,
            _ => throw new ArgumentOutOfRangeException(nameof(gtType))
        };
    }
}