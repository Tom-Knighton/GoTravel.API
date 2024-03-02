using GoTravel.API.Domain.Exceptions;
using GoTravel.API.Domain.Models.Database;
using GoTravel.API.Domain.Models.DTOs;
using GoTravel.API.Domain.Models.DTOs.Commands;
using GoTravel.API.Domain.Services;
using GoTravel.API.Domain.Services.Mappers;
using GoTravel.API.Domain.Services.Repositories;
using Microsoft.Extensions.Logging;

namespace GoTravel.API.Services.Services;

public class FriendshipService(IUserRepository repo, IMapper<GTUserDetails, UserDto> map, ILogger<FriendshipService> log, TimeProvider time): IFriendshipsService
{
    public async Task<IEnumerable<UserDto>> GetFollowersOfUser(string userId, bool includeNotYetAccepted = false, CancellationToken ct = default)
    {
        var follows = await repo.GetFollowersOfId(userId, includeNotYetAccepted, ct);
        var users = follows
            .Where(f => f.DoesFollow && (includeNotYetAccepted || f.IsAccepted))
            .Select(f => map.Map(f.Requester));

        return users.ToList();
    }

    public async Task<int> GetFollowerCountOfUser(string userId, bool includeNotYetAccepted = false, CancellationToken ct = default)
    {
        var followCount = await repo.GetFollowerCountOfId(userId, includeNotYetAccepted, ct);

        return followCount;
    }

    public async Task<IEnumerable<UserDto>> GetUsersFollowedBy(string userId, bool includeNotYetAccepted = false, CancellationToken ct = default)
    {
        var following = await repo.GetFollowingForUser(userId, includeNotYetAccepted, ct);
        var users = following
            .Where(f => f.DoesFollow && (includeNotYetAccepted || f.IsAccepted))
            .Select(f => map.Map(f.Requester));

        return users.ToList();

    }

    public async Task<bool> UpdateRelationship(string userId, SetRelationshipCommand command,  CancellationToken ct = default)
    {
        var user = await repo.GetUserByAnIdentifierAsync(command.FollowingId, ct);
        if (user is null)
        {
            throw new UserNotFoundException(userId);
        }
        
        var following = await repo.GetFollowing(user.UserId, command.FollowingId, ct);
        if (following is null)
        {
            following = new GTUserFollowings
            {
                RequesterId = userId,
                FollowsId = user.UserId,
                Created = time.GetUtcNow().UtcDateTime
            };
        }

        following.DoesFollow = command.Follow;
        following.IsAccepted = await repo.GetFollowAcceptTypeForUser(user.UserId, ct) switch
        {
            GTUserFollowerAcceptLevel.AcceptAll => true,
            GTUserFollowerAcceptLevel.RequiresApproval => false,
            GTUserFollowerAcceptLevel.NoFollowers => throw new NotAcceptingFollowersException(userId, command.FollowingId),
            _ => throw new ArgumentOutOfRangeException(nameof(following.IsAccepted))
        };

        return await repo.SaveRelationship(following, ct);
    }

    public async Task<bool> ApproveRejectRelationship(string userId, ApproveRejectFollowCommand command, CancellationToken ct = default)
    {
        var user = await repo.GetUserByAnIdentifierAsync(command.UserId, ct);
        if (user is null)
        {
            throw new UserNotFoundException(userId);
        }
        var following = await repo.GetFollowing(user.UserId, userId, ct);
        if (following is null)
        {
            throw new NoRelationshipException(user.UserId, userId);
        }

        if (following.FollowsId != userId)
        {
            throw new WrongUserException(following.FollowsId);
        }

        following.IsAccepted = command.Approve;
        if (!command.Approve)
        {
            following.DoesFollow = false;
        }

        return await repo.SaveRelationship(following, ct);
    }
}