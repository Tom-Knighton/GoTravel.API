using GoTravel.API.Domain.Data;
using GoTravel.API.Domain.Models.Database;
using GoTravel.API.Domain.Services.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GoTravel.API.Services.Services.Repositories;

public class UserRepository(GoTravelContext context, TimeProvider time): IUserRepository
{
    public async Task<GTUserDetails?> GetUserByIdAsync(string id, CancellationToken ct = default)
    {
        return await context
            .Users
            .IncludeFollowers(includeNotYetAccepted: true)
            .IncludeFollowing(includeNotYetAccepted: true)
            .IncludeSubtitles(time.GetUtcNow().UtcDateTime)
            .AsNoTrackingWithIdentityResolution()
            .FirstOrDefaultAsync(u => u.UserId == id, cancellationToken: ct);
    }

    public async Task<GTUserDetails?> GetUserByAnIdentifierAsync(string identifier, CancellationToken ct = default)
    {
        var user = await context
            .Users
            .IncludeFollowers()
            .AsNoTrackingWithIdentityResolution()
            .IncludeSubtitles(time.GetUtcNow().UtcDateTime)
            .FirstOrDefaultAsync(u => u.UserName == identifier || u.UserId == identifier, cancellationToken: ct);

        return user;
    }

    public async Task SaveUser(GTUserDetails userDetails, CancellationToken ct = default)
    {
        if (context.Users.Any(u => u.UserId == userDetails.UserId))
        {
            context.Users.Update(userDetails);
        }
        else
        {
            context.Users.Add(userDetails);
        }

        await context.SaveChangesAsync(ct);
    }

    public async Task<ICollection<GTUserDetails>> Search(string query, int maxResults, string? ignoreId, CancellationToken ct = default)
    {
        var results = await context.Users
            .IncludeSubtitles(time.GetUtcNow().UtcDateTime)
            .Where(u => EF.Functions.ILike(u.UserName, $"%{query}%") && u.UserId != ignoreId)
            .ToListAsync(ct);

        return results;
    }

    public async Task<GTUserFollowerAcceptLevel> GetFollowAcceptTypeForUser(string id, CancellationToken ct = default)
    {
        var type = await context.Users
            .Where(u => u.UserId == id)
            .Select(u => u.FollowerAcceptType)
            .FirstOrDefaultAsync(ct);

        return type;
    }

    public async Task<ICollection<GTUserFollowings>> GetFollowersOfId(string id, bool includeNotYetAccepted = false, CancellationToken ct = default)
    {
        var followers = await context.UserFollowings
            .AsNoTrackingWithIdentityResolution()
            .Where(f => f.FollowsId == id && f.DoesFollow && (includeNotYetAccepted || f.IsAccepted))
            .ToListAsync(ct);

        return followers;
    }

    public async Task<int> GetFollowerCountOfId(string id, bool includeNotYetAccepted = false, CancellationToken ct = default)
    {
        var followerCount = await context.UserFollowings
            .AsNoTrackingWithIdentityResolution()
            .Where(f => f.FollowsId == id && f.DoesFollow && (includeNotYetAccepted || f.IsAccepted))
            .CountAsync(ct);

        return followerCount;
    }

    public async Task<ICollection<GTUserFollowings>> GetFollowingForUser(string id, bool includeNotYetAccepted = false, CancellationToken ct = default)
    {
        var following = await context.UserFollowings
            .AsNoTrackingWithIdentityResolution()
            .Where(f => f.RequesterId == id && f.DoesFollow && (includeNotYetAccepted || f.IsAccepted))
            .ToListAsync(ct);

        return following;
    }

    public async Task<GTUserFollowings?> GetFollowing(string requesterId, string followingId, CancellationToken ct = default)
    {
        var following = await context.UserFollowings
            .AsNoTrackingWithIdentityResolution()
            .FirstOrDefaultAsync(f => f.RequesterId == requesterId && f.FollowsId == followingId, cancellationToken: ct);

        return following;
    }

    public async Task<bool> SaveRelationship(GTUserFollowings following, CancellationToken ct = default)
    {
        if (context.UserFollowings.AsNoTrackingWithIdentityResolution().Any(f => f.RequesterId == following.RequesterId && f.FollowsId == following.FollowsId))
        {
            context.UserFollowings.Update(following);
        }
        else
        {
            context.UserFollowings.Add(following);
        }

        return await context.SaveChangesAsync(ct) > 0;
    }

    public async Task AddUserAudit(GTUserPointsAudit audit, CancellationToken ct = default)
    {
        context.UserPointsAudit.Add(audit);
        await context.SaveChangesAsync(ct);
    }

    public async Task SaveTitle(string userId, GTUserSubtitle subtitle, CancellationToken ct = default)
    {
        context.UserSubtitles
            .Add(subtitle);

        await context.SaveChangesAsync(ct);
    }
}

public static class UserRepositoryExtensions
{
    public static IQueryable<GTUserDetails> IncludeFollowers(this IQueryable<GTUserDetails> query, bool includeNotYetAccepted = false)
    {
        return query
            .Include(x => x.Followers.Where(f => f.DoesFollow && (includeNotYetAccepted || f.IsAccepted )))
            .ThenInclude(f => f.Requester);
    }
    
    public static IQueryable<GTUserDetails> IncludeFollowing(this IQueryable<GTUserDetails> query, bool includeNotYetAccepted = false)
    {
        return query
            .Include(x => x.FollowingUsers.Where(f => f.DoesFollow && (includeNotYetAccepted || f.IsAccepted )))
            .ThenInclude(f => f.Follows);
    }

    public static IQueryable<GTUserDetails> IncludeSubtitles(this IQueryable<GTUserDetails> query, DateTime now)
    {
        return query
            .Include(u => u.Subtitles.Where(s => s.IsActive && s.ExpiresAt > now));
    }
}