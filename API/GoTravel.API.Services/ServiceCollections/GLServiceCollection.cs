using GoTravel.API.Domain.Services;
using GoTravel.API.Services.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GoTravel.API.Services.ServiceCollections;

public static class GLServiceCollection
{
    public static IServiceCollection AddGLServiceCollection(this IServiceCollection services)
    {
        services.AddTransient<IStopPointService, StopPointService>();
        services.AddTransient<ILineModeService, LineModeService>();
        
        return services;
    }
}