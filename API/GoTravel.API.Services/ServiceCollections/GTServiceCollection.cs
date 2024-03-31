using GoTravel.API.Domain.Services;
using GoTravel.API.Domain.Services.Auth;
using GoTravel.API.Services.Services;
using GoTravel.API.Services.Services.Auth;
using Microsoft.Extensions.DependencyInjection;

namespace GoTravel.API.Services.ServiceCollections;

public static class GTServiceCollection
{
    public static IServiceCollection AddGTServiceCollection(this IServiceCollection services)
    {
        services.AddTransient<IStopPointService, StopPointService>();
        services.AddTransient<ILineModeService, LineModeService>();
        services.AddTransient<IArrivalsService, ArrivalsService>();
        services.AddTransient<IJourneyService, JourneyService>();
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<IFriendshipsService, FriendshipService>();
        services.AddTransient<IPointsService, PointsService>();
        services.AddTransient<ICrowdsourceService, CrowdsourceService>();
        services.AddTransient<IScoreboardService, ScoreboardService>();
        services.AddTransient<ITripService, TripService>();
        
        return services;
    }
}