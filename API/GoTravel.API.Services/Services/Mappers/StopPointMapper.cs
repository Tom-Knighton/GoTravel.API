using GoTravel.API.Domain.Models.Database;
using GoTravel.API.Domain.Models.DTOs;
using GoTravel.API.Domain.Services.Mappers;

namespace GoTravel.API.Services.Services.Mappers;

public class StopPointMapper: IMapper<GLStopPoint, StopPointBaseDto>
{

    private IMapper<GLLineMode, LineModeDto> _lineModeMapper;

    public StopPointMapper(IMapper<GLLineMode, LineModeDto> lineModeMapper)
    {
        _lineModeMapper = lineModeMapper;
    }
    
    public StopPointBaseDto Map(GLStopPoint src)
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
            GLStopPointType.TrainStopPoint => MapAsTrainStop(src, dest),
            GLStopPointType.BusStopPoint => MapAsBusStop(src, dest),
            GLStopPointType.BikeStopPoint => MapAsBikeStop(src, dest),
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
            mode.Lines = lines.Where(l => l.LineModeId == mode.LineModeName).ToList();
            var newMode = _lineModeMapper.Map(mode);
            newLineModes.Add(newMode);
        }

        dest.LineModes = newLineModes;

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

    private StopPointType MapType(GLStopPointType glType)
    {
        return glType switch
        {
            GLStopPointType.TrainStopPoint => StopPointType.Train,
            GLStopPointType.BusStopPoint => StopPointType.Bus,
            GLStopPointType.BikeStopPoint => StopPointType.Bike,
            _ => throw new ArgumentOutOfRangeException(nameof(glType))
        };
    }
}