using GoTravel.API.Domain.Models.Lib;
using GoTravel.API.Domain.Services;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace GoTravel.API.Services.Consumers;

public class IAddPointsConsumer: IConsumer<AddPointsMessage>
{
    private readonly IPointsService _points;
    private readonly ILogger<IAddPointsConsumer> _log;

    public IAddPointsConsumer(IPointsService pointsService, ILogger<IAddPointsConsumer> log)
    {
        _points = pointsService;
        _log = log;
    }
    
    public async Task Consume(ConsumeContext<AddPointsMessage> context)
    {
        _log.LogInformation("Adding {Points} to {User} for {Reason}", context.Message.Points, context.Message.UserId, context.Message.Message);
        await _points.AddPointsToUser(context.Message.UserId, (int)context.Message.Points, context.Message.Message, context.Message.ReasonType);
    }
}