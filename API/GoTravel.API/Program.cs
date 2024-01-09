using System.Reflection;
using System.Text.Json.Serialization;
using GoTravel.API.Domain.Data;
using GoTravel.API.Services.ServiceCollections;
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
});
builder.Services.AddSingleton(NtsGeometryServices.Instance);

builder.Services
    .AddSwaggerServices(builder.Configuration.GetSection("Authentication"))
    .AddAuthServices(builder.Configuration.GetSection("Authentication"))
    .AddEFCore<GoTravelContext>(builder.Configuration.GetSection("Database"))
    .AddMapperCollection()
    .AddRepositories()
    .AddGLServiceCollection()
    .AddRabbitMq(builder.Configuration.GetSection("Rabbit"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwaggerServices(builder.Configuration.GetSection("Authentication"));
}

app.UseAuthServices();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();