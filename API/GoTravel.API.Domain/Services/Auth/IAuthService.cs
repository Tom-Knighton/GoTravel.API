using GoTravel.API.Domain.Models.Lib;

namespace GoTravel.API.Domain.Services.Auth;

public interface IAuthService
{
    public Task<AuthUserInfoResponse?> GetCurrentUserInfo(CancellationToken ct = default);
}