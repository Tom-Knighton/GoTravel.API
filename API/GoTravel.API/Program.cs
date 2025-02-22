using System.Reflection;
using System.Text.Json.Serialization;
using GoTravel.API.Domain.Data;
using GoTravel.API.Services.ServiceCollections;
using IdempotentAPI.Cache.DistributedCache.Extensions.DependencyInjection;
using IdempotentAPI.Extensions.DependencyInjection;
using NetTopologySuite;
using NetTopologySuite.IO.Converters;

var builder = WebApplication.CreateBuilder(args);

var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile("appsettings." + environmentName + ".json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets(Assembly.GetExecutingAssembly());
}

builder.Services.AddControllers().AddJsonOptions(o =>
{
    var geoJsonConverterFactory = new GeoJsonConverterFactory();
    o.JsonSerializerOptions.Converters.Add(geoJsonConverterFactory);
    o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
builder.Services.AddSingleton(NtsGeometryServices.Instance);

builder.Services
    .AddHttpContextAccessor()
    .AddSwaggerServices(builder.Configuration.GetSection("Authentication"))
    .AddAuthServices(builder.Configuration.GetSection("Authentication"))
    .AddEFCore<GoTravelContext>(builder.Configuration.GetSection("Database"))
    .AddMapperCollection()
    .AddRepositories()
    .AddGTServiceCollection()
    .AddRabbitMq(builder.Configuration.GetSection("Rabbit"))
    .AddConnectorServices(builder.Configuration.GetSection("Connector"))
    .AddRedis(builder.Configuration.GetSection("Redis"))
    .AddMinioCollection(builder.Configuration.GetSection("CDN"))
    .AddLogs()
    .AddDistributedMemoryCache()
    .AddIdempotentAPI()
    .AddIdempotentAPIUsingDistributedCache()
    .AddHangfireCollection(builder.Configuration.GetSection("Hangfire"))
    .ConfigureHttpJsonOptions(o =>
    {
        o.SerializerOptions.PropertyNameCaseInsensitive = true;
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwaggerServices(builder.Configuration.GetSection("Authentication"));
}

app.UseRouting();
app.UseAuthServices();
app.UseHangfire(builder.Configuration.GetSection("Hangfire"));
app.UseHttpsRedirection();
app.MapControllers();

app.UseEfCore();

app.Run();

