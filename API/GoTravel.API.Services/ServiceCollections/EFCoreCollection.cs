using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

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
            });
        });

        return services;
    }
}