using System.Text.Json;

namespace GoTravel.API.Domain.Singletons;

public static class JsonSingleton
{
    public static readonly JsonSerializerOptions _options = new(JsonSerializerDefaults.Web)
    {
        //Can add custom overrides here...
    };

    public static JsonSerializerOptions Options => _options;
}