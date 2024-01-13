using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GoTravel.API.Services.ServiceCollections;

public static class ConnectorCollection
{
    public static IServiceCollection AddConnectorServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient("GTCON", c =>
        {
            c.BaseAddress = new Uri(configuration["BaseUrl"]);
            
            c.DefaultRequestHeaders.Add("User-Agent", "GoTravel.API");
        });
        
        
        return services;
    }
}