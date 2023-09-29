using GoLondon.API.Domain.Models.Database;
using GoLondon.API.Domain.Models.DTOs;
using GoLondon.API.Domain.Services.Mappers;
using GoLondon.API.Services.Services.Mappers;
using Microsoft.Extensions.DependencyInjection;

namespace GoLondon.API.Services.ServiceCollections;

public static class MapperCollection
{
    public static IServiceCollection AddMapperCollection(this IServiceCollection services)
    {
        services.AddTransient<IMapper<GLStopPoint, StopPointBaseDto>, StopPointMapper>();

        return services;
    }
}