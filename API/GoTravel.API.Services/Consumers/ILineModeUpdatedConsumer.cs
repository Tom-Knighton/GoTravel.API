using GoTravel.API.Domain.Services;
using GoTravel.Standard.Messages;
using MassTransit;

namespace GoTravel.API.Services.Consumers;

public class ILineModeUpdatedConsumer: IConsumer<ILineModeUpdated>
{
    private readonly ILineModeService _lineModeService;

    public ILineModeUpdatedConsumer(ILineModeService lineModeService)
    {
        _lineModeService = lineModeService;
    } 
    public async Task Consume(ConsumeContext<ILineModeUpdated> context)
    {
        try
        {
            var update = context.Message.Dto;
            if (string.IsNullOrWhiteSpace(update.LineModeName))
            {
                throw new ArgumentNullException(nameof(update.LineModeName),
                    "LineMode name cannot be null when updating");
            }

            if (update.Lines?.Any() == false)
            {
                throw new ArgumentNullException(nameof(update.Lines), "Lines are empty or null trying to update");
            }

            await _lineModeService.UpdateLineMode(context.Message.Dto);
        }
        catch (Exception ex)
        {
            //TODO: log
            throw;
        }
    }
}