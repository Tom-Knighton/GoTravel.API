using GoTravel.API.Domain.Models.Database;
using GoTravel.API.Domain.Models.DTOs;
using GoTravel.API.Domain.Models.Lib;
using GoTravel.API.Domain.Services.Mappers;
using GoTravel.API.Services.Services.Mappers;
using GoTravel.Standard.Models.Arrivals;
using GoTravel.Standard.Models.Journeys;
using GoTravel.Standard.Models.MessageModels;
using Microsoft.Extensions.DependencyInjection;

namespace GoTravel.API.Services.ServiceCollections;

public static class MapperCollection
{
    public static IServiceCollection AddMapperCollection(this IServiceCollection services)
    {
        services.AddTransient<IMapper<GTStopPoint, StopPointBaseDto>, StopPointMapper>();
        services.AddTransient<IMapper<GLLineMode, LineModeDto>, LineModeMapper>();
        services.AddTransient<IMapper<GLFlag, string>, FlagsMapper>();
        services.AddTransient<IMapper<StopPointUpdateDto, GTStopPoint>, StopPointUpdateMapper>();
        services.AddTransient<IMapper<IEnumerable<ArrivalDeparture>, ICollection<LineArrivals>>, ArrivalGroupMapper>();
        services.AddTransient<IMapper<ICollection<GTStopPointInfoValue>, StopPointInformationDto>, StopPointInfoMapper>();
        services.AddTransient<IMapper<Tuple<GTUserDetails, AuthUserInfoResponse>, CurrentUserDto>, CurrentUserMapper>();
        services.AddTransient<IMapper<GTUserDetails, UserDto>, UserMapper>();
        services.AddTransient<IMapper<GTCrowdsourceInfo, CrowdsourceInfoDto>, CrowdsourceMapper>();
        services.AddTransient<IMapper<GTScoreboard, ScoreboardDto>, ScoreboardMapper>();
        
        services.AddTransient<IAsyncMapper<Journey, JourneyDto>, JourneyAsyncMapper>();
        return services;
    }
}