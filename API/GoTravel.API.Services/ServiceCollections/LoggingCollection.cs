using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace GoTravel.API.Services.ServiceCollections;

public static class LoggingCollection
{
    public static IServiceCollection AddLogs(this IServiceCollection services)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

        return services;
    }
}