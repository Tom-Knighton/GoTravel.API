using GoLondon.API.Domain.Services.Repositories;
using GoLondon.API.Services.Services.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace GoLondon.API.Services.ServiceCollections;

public static class GLRepositoryCollection
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddTransient<IStopPointRepository, StopPointRepository>();

        return services;
    }
}