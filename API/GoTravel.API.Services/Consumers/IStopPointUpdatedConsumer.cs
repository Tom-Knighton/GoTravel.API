using GoTravel.API.Domain.Services;
using GoTravel.Standard.Messages;
using MassTransit;

namespace GoTravel.API.Services.Consumers;

public class IStopPointUpdatedConsumer: IConsumer<IStopPointUpdated>
{
    private readonly IStopPointService _stopPointService;

    public IStopPointUpdatedConsumer(IStopPointService stopPointService)
    {
        _stopPointService = stopPointService;
    }
    
    
    public async Task Consume(ConsumeContext<IStopPointUpdated> context)
    {
        try
        {
            var update = context.Message.Dto;
            if (update.Latitude is null || update.Longitude is null)
            {
                throw new ArgumentNullException(nameof(update.Latitude),
                    "Longitude and Latitude cannot be null when updating");
            }

            await _stopPointService.UpdateStopPoint(context.Message.Dto);
        }
        catch (Exception ex)
        {
            //TODO: log
            throw;
        }
    }
}