using System.Collections;
using GoTravel.API.Domain.Models.DTOs;
using GoTravel.API.Domain.Models.DTOs.Commands;

namespace GoTravel.API.Domain.Services;

public interface IFriendshipsService
{
    public Task<IEnumerable<UserDto>> GetFollowersOfUser(string userId, bool includeNotYetAccepted = false, CancellationToken ct = default);
    public Task<int> GetFollowerCountOfUser(string userId, bool includeNotYetAccepted = false, CancellationToken ct = default);
    public Task<IEnumerable<UserDto>> GetUsersFollowedBy(string userId, bool includeNotYetAccepted = false, CancellationToken ct = default);

    public Task<bool> UpdateRelationship(string userId, SetRelationshipCommand command, CancellationToken ct = default);
    public Task<bool> ApproveRejectRelationship(string userId, ApproveRejectFollowCommand command, CancellationToken ct = default);
}