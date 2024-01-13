using GoTravel.API.Domain.Models.Database;
using GoTravel.API.Domain.Models.DTOs;
using GoTravel.API.Domain.Services.Mappers;
using GoTravel.API.Services.Services.Mappers;
using GoTravel.Standard.Models.Arrivals;
using GoTravel.Standard.Models.MessageModels;
using Microsoft.Extensions.DependencyInjection;

namespace GoTravel.API.Services.ServiceCollections;

public static class MapperCollection
{
    public static IServiceCollection AddMapperCollection(this IServiceCollection services)
    {
        services.AddTransient<IMapper<GLStopPoint, StopPointBaseDto>, StopPointMapper>();
        services.AddTransient<IMapper<GLLineMode, LineModeDto>, LineModeMapper>();
        services.AddTransient<IMapper<GLFlag, string>, FlagsMapper>();
        services.AddTransient<IMapper<StopPointUpdateDto, GLStopPoint>, StopPointUpdateMapper>();
        services.AddTransient<IMapper<IEnumerable<ArrivalDeparture>, ICollection<LineArrivals>>, ArrivalGroupMapper>();
        return services;
    }
}