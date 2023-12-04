using GoTravel.API.Domain.Services.Repositories;
using GoTravel.API.Services.Services.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace GoTravel.API.Services.ServiceCollections;

public static class GLRepositoryCollection
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddTransient<IStopPointRepository, StopPointRepository>();

        return services;
    }
}