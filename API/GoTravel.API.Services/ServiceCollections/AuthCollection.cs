using System.Security.Claims;
using System.Text.Encodings.Web;
using GoTravel.API.Domain.Services.Auth;
using GoTravel.API.Domain.Services.Repositories;
using GoTravel.API.Services.Services.Auth;
using GoTravel.API.Services.Services.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
            .AddScheme<AuthenticationSchemeOptions, APIAuthSchemeHandler>("Auth0Only", null)
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


        services.AddHttpClient("Auth0", c =>
        {
            c.BaseAddress = new Uri(configuration["Auth0BaseUrl"]);
            c.DefaultRequestHeaders.Add("User-Agent", "GoTravel.API");
        });

        services.AddSingleton<IM2MService, M2MService>();
        services.AddTransient<IUserRepository, UserRepository>();
        services.AddTransient<IAuthService, AuthService>();
        services.AddTransient<IUserService, UserService>();
        
        return services;
    }

    public static void UseAuthServices(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
    }
}

public class APIAuthSchemeHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private const string ApiKeyHeaderName = "X-Api-Key";
    private readonly string _apiKey;

    public APIAuthSchemeHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options, 
        ILoggerFactory logger, 
        UrlEncoder encoder, 
        ISystemClock clock,
        IConfiguration configuration) 
        : base(options, logger, encoder, clock)
    {
        _apiKey = configuration.GetValue<string>("Authentication:PSSecret");
    }


    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(ApiKeyHeaderName, out var apiKeyHeaderValues))
        {
            return AuthenticateResult.NoResult();
        }

        var providedApiKey = apiKeyHeaderValues.FirstOrDefault();

        if (!string.IsNullOrWhiteSpace(_apiKey) && providedApiKey == _apiKey)
        {
            var claims = new[] { new Claim(ClaimTypes.Name, "ApiKeyUser") };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }

        return AuthenticateResult.Fail("Invalid API Key provided.");
    }
}