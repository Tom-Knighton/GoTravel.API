using GoTravel.API.Domain.Services;
using GoTravel.Standard.Messages;
using MassTransit;

namespace GoTravel.API.Services.Consumers;

public class IStopPointInfoUpdatedConsumer: IConsumer<IStopPointInfoUpdated>
{
    private readonly IStopPointService _stopPointService;

    public IStopPointInfoUpdatedConsumer(IStopPointService stopPointService)
    {
        _stopPointService = stopPointService;
    }
    
    public async Task Consume(ConsumeContext<IStopPointInfoUpdated> context)
    {
        try
        {
            var stopId = context.Message.StopPointId;
            var infoKvps = context.Message.Infos;
            await _stopPointService.ClearAndUpdateStopPointInfo(stopId, infoKvps, default);
        }
        catch (Exception ex)
        {
            //TODO: log
            throw;
        }
    }
}