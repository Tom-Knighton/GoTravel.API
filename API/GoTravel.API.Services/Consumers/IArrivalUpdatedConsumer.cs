using GoTravel.Standard.Messages;
using MassTransit;

namespace GoTravel.API.Services.Consumers;

public class IArrivalUpdatedConsumer: IConsumer<IArrivalUpdated>
{
    public Task Consume(ConsumeContext<IArrivalUpdated> context)
    {
        return Task.FromResult(0);
    }
}