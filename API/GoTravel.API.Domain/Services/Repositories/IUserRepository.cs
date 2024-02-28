using GoTravel.API.Domain.Models.Database;

namespace GoTravel.API.Domain.Services.Repositories;

public interface IUserRepository
{
    Task<GTUserDetails?> GetUserByIdAsync(string id, CancellationToken ct = default);
    Task<GTUserDetails?> GetUserByAnIdentifierAsync(string identifier, CancellationToken ct = default);
    Task SaveUser(GTUserDetails userDetails, CancellationToken ct = default);


    Task<GTUserFollowerAcceptLevel> GetFollowAcceptTypeForUser(string id, CancellationToken ct = default);
    Task<ICollection<GTUserFollowings>> GetFollowersOfId(string id, bool includeNotYetAccepted = false, CancellationToken ct = default);
    Task<int> GetFollowerCountOfId(string id, bool includeNotYetAccepted = false, CancellationToken ct = default);
    Task<ICollection<GTUserFollowings>> GetFollowingForUser(string id, bool includeNotYetAccepted = false, CancellationToken ct = default);

    Task<GTUserFollowings?> GetFollowing(string requesterId, string followingId, CancellationToken ct = default);
    Task<bool> SaveRelationship(GTUserFollowings following, CancellationToken ct = default);

}