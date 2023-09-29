using GoLondon.API.Domain.Services;
using GoLondon.API.Services.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GoLondon.API.Services.ServiceCollections;

public static class GLServiceCollection
{
    public static IServiceCollection AddGLServiceCollection(this IServiceCollection services)
    {
        services.AddTransient<IStopPointService, StopPointService>();

        return services;
    }
}