using GoTravel.API.Domain.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Zomp.EFCore.WindowFunctions.Npgsql;

namespace GoTravel.API.Services.ServiceCollections;

public static class EFCoreCollection
{

    public static IServiceCollection AddEFCore<T>(this IServiceCollection services, IConfiguration configuration) where T: DbContext
    {
        var builder = new NpgsqlConnectionStringBuilder
        {   
            Host = configuration["Host"],
            Username = configuration["Username"],
            Password = configuration["Password"],
            Port = Convert.ToInt32(configuration["Port"] ?? "5432"),
            Database = configuration["Username"],
            Pooling = true,
            MinPoolSize = 0,
            MaxPoolSize = 100
        };
        
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehaviour", true);
        services.AddDbContext<T>(o =>
        {
            o.UseNpgsql(builder.ConnectionString, b =>
            {
                b.MigrationsAssembly("GoTravel.API");
                b.UseNetTopologySuite();
                b.UseWindowFunctions();
            });
        });

        services.AddTransient<GoTravelSeeder>();
        
        return services;
    }

    public static IApplicationBuilder UseEfCore(this IApplicationBuilder app)
    {
        var scopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();

        using var scope = scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<GoTravelSeeder>();
        service.Seed();
        
        return app;
    }
}