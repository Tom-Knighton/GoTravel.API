using GoTravel.API.Domain.Services.Repositories;
using GoTravel.API.Services.Services.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace GoTravel.API.Services.ServiceCollections;

public static class GTRepositoryCollection
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddTransient<IStopPointRepository, StopPointRepository>();
        services.AddTransient<IAreaRepository, AreaRepository>();
        services.AddTransient<ILineModeRepository, LineModeRepository>();
        services.AddTransient<ICrowdsourceRepository, CrowdsourceRepository>();
        services.AddTransient<IScoreboardRepository, ScoreboardRepository>();
        
        return services;
    }
}