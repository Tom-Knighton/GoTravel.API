using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GoTravel.API.Services.ServiceCollections;

public static class AuthCollection
{
    public static IServiceCollection AddAuthServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Authority = configuration["Authority"];
                options.Audience = configuration["Audience"];
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("ManageLines", p => p.RequireClaim("permissions", "manage:lines"));
            options.AddPolicy("ManageStops", p => p.RequireClaim("permissions", "manage:stops"));
        });
        return services;
    }

    public static void UseAuthServices(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
    }
}