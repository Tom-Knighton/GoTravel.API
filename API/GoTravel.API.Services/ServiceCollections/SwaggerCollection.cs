using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace GoTravel.API.Services.ServiceCollections;

public static class SwaggerCollection
{
    public static IServiceCollection AddSwaggerServices(this IServiceCollection services, IConfiguration authConfig)
    {
        var domain = authConfig["Authority"];
        var audience = authConfig["Audience"];
        
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("OAuth", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    Implicit = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri($"{domain}authorize?audience={audience}"),
                        Scopes = new Dictionary<string, string>
                        {
                            { "openid", "Basic User Details"},
                            { "profile", "Basic User Details"},
                            { "https://gotravel/nickname", "Custom nickname"},
                            { "preferred_username", "Auth0 nickname"},
                            { "offline_access", "Refresh Token"},
                        },
                    }
                }
            });
    
            c.OperationFilter<SecurityRequirementsOperationFilter>();
        });

        return services;
    }

    public static void UseSwaggerServices(this IApplicationBuilder app, IConfiguration authConfig)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.OAuthClientId(authConfig["SwaggerClientId"]);
        });
    }
}

public class SecurityRequirementsOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var hasAllowAnonymous = context.MethodInfo
            .GetCustomAttributes(true)
            .OfType<AllowAnonymousAttribute>()
            .Any();

        if (hasAllowAnonymous)
        {
            return;
        }
        
        var methodAuthorizeAttributes = context.MethodInfo
            .GetCustomAttributes(true)
            .OfType<AuthorizeAttribute>();

        var controllerAuthorizeAttributes = context.MethodInfo.DeclaringType
            .GetCustomAttributes(true)
            .OfType<AuthorizeAttribute>();

        var allAuthorizeAttributes = methodAuthorizeAttributes.Concat(controllerAuthorizeAttributes)
            .Distinct()
            .ToList();

        var requiredScopes = allAuthorizeAttributes
            .Select(attr => attr.Policy)
            .Distinct()
            .ToList();

        if (requiredScopes.Count != 0)
        {
            operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
            operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });
            
            var oAuthScheme = new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "OAuth" }
            };

            operation.Security = new List<OpenApiSecurityRequirement>
            {
                new()
                {
                    [ oAuthScheme ] = requiredScopes.ToList()
                }
            };
        }
    }
}