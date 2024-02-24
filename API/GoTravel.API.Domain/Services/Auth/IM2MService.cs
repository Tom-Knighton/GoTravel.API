namespace GoTravel.API.Domain.Services.Auth;

public interface IM2MService
{
    public Task<string> GetM2MToken(CancellationToken ct = default);
}