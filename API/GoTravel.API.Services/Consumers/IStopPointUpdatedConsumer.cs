using GoTravel.Standard.Messages;
using MassTransit;

namespace GoTravel.API.Services.Consumers;

public class IStopPointUpdatedConsumer: IConsumer<IStopPointUpdated>
{
    public Task Consume(ConsumeContext<IStopPointUpdated> context)
    {
        return Task.FromResult(0);
    }
}