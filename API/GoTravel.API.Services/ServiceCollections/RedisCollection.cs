using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace GoTravel.API.Services.ServiceCollections;

public static class RedisCollection
{
    public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
    {
        var host = configuration["Host"];
        var port = int.Parse(configuration["Port"] ?? "6379");
        var db = configuration["Database"];
        var pass = configuration["Password"];
        
        var options = new ConfigurationOptions()
        {
            EndPoints = { { host, port } },
            Password = pass,
            DefaultDatabase = int.Parse(db),
            ClientName = "GTAPI",
        };

        var multiplexer = ConnectionMultiplexer.Connect(options);
        services.AddScoped<IDatabase>(_ => multiplexer.GetDatabase(int.Parse(db)));
        
        return services;
    }
}