using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using GoTravel.API.Domain.Services.Auth;
using GoTravel.API.Domain.Singletons;
using Microsoft.Extensions.Configuration;

namespace GoTravel.API.Services.Services.Auth;

public class M2MService: IM2MService
{
    private HttpClient _client;
    private string? _token;
    private DateTime? _expiry;
    private IConfiguration _authSection;

    public M2MService(IHttpClientFactory factory, IConfiguration configuration)
    {
        _client = factory.CreateClient("Auth0");
        _authSection = configuration.GetSection("Authentication");
    }
    
    public async Task<string> GetM2MToken(CancellationToken ct = default)
    {
        if (!string.IsNullOrWhiteSpace(_token) && DateTime.UtcNow < _expiry)
        {
            return _token;
        }
        
        using var request = new HttpRequestMessage(HttpMethod.Get, "oauth/token");
        request.Method = HttpMethod.Post;

        var content = new
        {
            client_id = _authSection.GetValue<string>("ManagementId"),
            client_secret = _authSection.GetValue<string>("ManagementSecret"),
            audience = _authSection.GetValue<string>("Authority") + "api/v2/",
            grant_type = "client_credentials"
        };

        request.Content = new StringContent(JsonSerializer.Serialize(content), Encoding.UTF8, "application/json");

        var response = await _client.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();

        var stream = await response.Content.ReadAsStreamAsync(ct);
        var jsonNode = await JsonSerializer.DeserializeAsync<JsonNode>(stream, JsonSingleton.Options, ct);

        _token = jsonNode["access_token"].GetValue<string>();

        var handler = new JwtSecurityTokenHandler();
        var decoded = handler.ReadJwtToken(_token);

        _expiry = decoded.ValidTo;

        return _token;
    }
}