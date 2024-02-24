using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using GoTravel.API.Domain.Models.Lib;
using GoTravel.API.Domain.Services.Auth;
using GoTravel.API.Domain.Singletons;
using Microsoft.AspNetCore.Http;

namespace GoTravel.API.Services.Services.Auth;

public class AuthService: IAuthService
{
    private readonly IM2MService _m2m;
    private readonly IHttpContextAccessor _context;
    private readonly HttpClient _client;
    
    public AuthService(IM2MService m2m, IHttpContextAccessor context, IHttpClientFactory httpClientFactory)
    {
        _m2m = m2m;
        _context = context;
        _client = httpClientFactory.CreateClient("Auth0");
    }
    
    public async Task<AuthUserInfoResponse?> GetCurrentUserInfo(CancellationToken ct = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "userinfo");
        var token = _context.HttpContext.Request.Headers.Authorization.ToString();
        if (token.StartsWith("Bearer", StringComparison.OrdinalIgnoreCase))
        {
            token = token["Bearer ".Length..].Trim();
        }
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request, ct);

        response.EnsureSuccessStatusCode();

        var stream = await response.Content.ReadAsStreamAsync(ct);
        var userInfo = await JsonSerializer.DeserializeAsync<AuthUserInfoResponse>(stream, JsonSingleton.Options, ct);

        return userInfo;
    }

    public async Task<bool> UpdateUsername(string username, string id, CancellationToken ct = default)
    {
        var token = await _m2m.GetM2MToken(ct);
        
        using var request = new HttpRequestMessage(HttpMethod.Get, $"api/v2/users/{id}");
        request.Method = HttpMethod.Patch;
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.Add("Accept", "application/json");
        var content = new
        {
            username = username
        };

        request.Content = new StringContent(JsonSerializer.Serialize(content), Encoding.UTF8, "application/json");
  
        var response = await _client.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();

        return true;
    }

    public async Task<bool> UpdateProfilePictureUrl(string url, string id, CancellationToken ct = default)
    {
        var token = await _m2m.GetM2MToken(ct);
        
        using var request = new HttpRequestMessage(HttpMethod.Get, $"api/v2/users/{id}");
        request.Method = HttpMethod.Patch;
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.Add("Accept", "application/json");
        var content = new
        {
            picture = url
        };

        request.Content = new StringContent(JsonSerializer.Serialize(content), Encoding.UTF8, "application/json");
        
        var response = await _client.SendAsync(request, ct);

        response.EnsureSuccessStatusCode();

        return true;
    }
}