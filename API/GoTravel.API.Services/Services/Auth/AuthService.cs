using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using GoTravel.API.Domain.Models.Lib;
using GoTravel.API.Domain.Services.Auth;
using GoTravel.API.Domain.Singletons;
using Microsoft.AspNetCore.Http;

namespace GoTravel.API.Services.Services.Auth;

public class AuthService: IAuthService
{
    private readonly IHttpContextAccessor _context;
    private readonly HttpClient _client;
    
    public AuthService(IHttpContextAccessor context, IHttpClientFactory httpClientFactory)
    {
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
}