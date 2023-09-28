using System.Reflection;
using GoLondon.API.Data;
using GoLondon.API.Services.ServiceCollections;
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
});
builder.Services.AddSingleton(NtsGeometryServices.Instance);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddEFCore<GoLondonContext>(builder.Configuration.GetSection("Database"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();