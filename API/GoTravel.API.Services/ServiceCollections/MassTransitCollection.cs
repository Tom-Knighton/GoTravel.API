using GoTravel.API.Services.Consumers;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GoTravel.API.Services.ServiceCollections;

public static class MassTransitCollection
{
    public static IServiceCollection AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(mt =>
        {

            mt.AddConsumer<IStopPointUpdatedConsumer>();
            mt.AddConsumer<ILineModeUpdatedConsumer>();
            mt.AddConsumer<IArrivalUpdatedConsumer>();
            
            mt.UsingRabbitMq((ctx, cfg) =>
            {
                var host = configuration["Host"];
                var vh = configuration["VirtualHost"];
                var port = ushort.Parse(configuration["Port"]);
                cfg.Host(host, port, vh, c =>
                {
                    var username = configuration["User"];
                    var password = configuration["Password"];
                    c.Username(username);
                    c.Password(password);
                });
                
                cfg.ReceiveEndpoint("IStopPointUpdated_GTAPI", c =>
                {
                    c.ConfigureConsumer<IStopPointUpdatedConsumer>(ctx);
                });
                
                cfg.ReceiveEndpoint("ILineModeUpdated_GTAPI", c =>
                {
                    c.ConfigureConsumer<ILineModeUpdatedConsumer>(ctx);
                });
                
                cfg.ReceiveEndpoint("IArrivalUpdated_GTAPI", c =>
                {
                    c.ConfigureConsumer<IArrivalUpdatedConsumer>(ctx);
                });
                
            });
        });

        return services;
    }
}