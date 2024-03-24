using GoTravel.API.Domain.Services;
using GoTravel.Standard.Messages;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace GoTravel.API.Services.Consumers;

public class ILineRouteUpdatedConsumer: IConsumer<ILineStringUpdated>
{
    private readonly ILineModeService _lineModeService;
    private readonly ILogger<ILineRouteUpdatedConsumer> _log;

    public ILineRouteUpdatedConsumer(ILineModeService lineModeService, ILogger<ILineRouteUpdatedConsumer> log)
    {
        _lineModeService = lineModeService;
        _log = log;
    }
    
    public async Task Consume(ConsumeContext<ILineStringUpdated> context)
    {
        try
        {
            await _lineModeService.UpdateLineRoute(context.Message.dto);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Failed to update line route {LineId}, Message: {@Message}", context.Message.dto.LineId, context.Message);
            throw;
        }
    }
}