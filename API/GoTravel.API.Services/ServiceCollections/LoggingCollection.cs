using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;

namespace GoTravel.API.Services.ServiceCollections;

public static class LoggingCollection
{
    public static IServiceCollection AddLogs(this IServiceCollection services)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .MinimumLevel.Override("Microsoft.AspNetCore.Hosting", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning)
            .CreateLogger();

        return services;
    }
}