using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;

namespace GoTravel.API.Services.ServiceCollections;

public static class MinioCollection
{
    public static IServiceCollection AddMinioCollection(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddMinio(o =>
        {
            o.WithEndpoint(configuration.GetValue<string>("Host"));
            o.WithCredentials(configuration.GetValue<string>("Access"), configuration.GetValue<string>("Secret"));
            o.Build();
        });

        return services;
    }
}