using GoTravel.API.Domain.Models.Database;

namespace GoTravel.API.Domain.Services.Repositories;

public interface IUserRepository
{
    Task<GTUserDetails?> GetUserByIdAsync(string id, CancellationToken ct = default);
    Task<GTUserDetails?> GetUserByAnIdentifierAsync(string identifier, CancellationToken ct = default);
    Task SaveUser(GTUserDetails userDetails, CancellationToken ct = default);
}