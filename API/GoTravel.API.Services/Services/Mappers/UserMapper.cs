using GoTravel.API.Domain.Models.Database;
using GoTravel.API.Domain.Models.DTOs;
using GoTravel.API.Domain.Models.Lib;
using GoTravel.API.Domain.Services.Mappers;

namespace GoTravel.API.Services.Services.Mappers;

public class UserMapper : IMapper<GTUserDetails, UserDto>
{
    public UserDto Map(GTUserDetails source)
    {
        var dto = new UserDto
        {
            UserName = source.UserName,
            UserPictureUrl = source.UserProfilePicUrl,
            FollowerCount = source.Followers?.Count ?? 0,
            UserPoints = source.UserPoints,
            Subtitle = source.Subtitles.FirstOrDefault()?.Title
        };

        return dto;
    }
}

public class CurrentUserMapper(IMapper<GTUserDetails, UserDto> basicMapper): IMapper<Tuple<GTUserDetails, AuthUserInfoResponse>, CurrentUserDto>
{
    public CurrentUserDto Map(Tuple<GTUserDetails, AuthUserInfoResponse> source)
    {
        var dto = new CurrentUserDto
        {
            UserId = source.Item1.UserId,
            UserName = source.Item1.UserName,
            UserEmail = source.Item2.name,
            DateCreated = source.Item1.DateCreated,
            UserPictureUrl = source.Item1.UserProfilePicUrl,
            Followers = MapFollowing(source.Item1.Followers, mapFollower: false),
            Following = MapFollowing(source.Item1.FollowingUsers),
            UserPoints = source.Item1.UserPoints,
            Subtitle = source.Item1.Subtitles.FirstOrDefault()?.Title
        };

        return dto;
    }
    
    private IEnumerable<UserFollowingDto> MapFollowing(IEnumerable<GTUserFollowings>? followings, bool mapFollower = true)
    {
        var following = followings?
            .Where(f => f.DoesFollow)
            .Select(f => new UserFollowingDto
            {
                FollowingType = f.IsAccepted ? FollowingType.Following : FollowingType.Requested,
                User = basicMapper.Map(mapFollower ? f.Follows : f.Requester)
            }
        );

        return following ?? new List<UserFollowingDto>();
    }
}