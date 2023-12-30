using GoTravel.Standard.Messages;
using MassTransit;

namespace GoTravel.API.Services.Consumers;

public class ILineModeUpdatedConsumer: IConsumer<ILineModeUpdated>
{
    public Task Consume(ConsumeContext<ILineModeUpdated> context)
    {
        return Task.FromResult(0);
    }
}