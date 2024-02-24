using GoTravel.API.Domain.Models.Lib;

namespace GoTravel.API.Domain.Services.Auth;

public interface IAuthService
{
    public Task<AuthUserInfoResponse?> GetCurrentUserInfo(CancellationToken ct = default);

    public Task<bool> UpdateUsername(string username, string id, CancellationToken ct = default);

    public Task<bool> UpdateProfilePictureUrl(string url, string id, CancellationToken ct = default);
}