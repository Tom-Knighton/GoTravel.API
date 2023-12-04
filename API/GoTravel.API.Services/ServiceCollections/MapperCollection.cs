using GoTravel.API.Domain.Models.Database;
using GoTravel.API.Domain.Models.DTOs;
using GoTravel.API.Domain.Services.Mappers;
using GoTravel.API.Services.Services.Mappers;
using Microsoft.Extensions.DependencyInjection;

namespace GoTravel.API.Services.ServiceCollections;

public static class MapperCollection
{
    public static IServiceCollection AddMapperCollection(this IServiceCollection services)
    {
        services.AddTransient<IMapper<GLStopPoint, StopPointBaseDto>, StopPointMapper>();

        return services;
    }
}