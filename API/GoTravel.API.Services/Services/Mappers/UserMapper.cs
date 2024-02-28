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
            FollowerCount = source.Followers?.Count ?? 0
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
            Followers = source.Item1.Followers?
                .Where(f => f is { DoesFollow: true, IsAccepted: true })
                .Select(f => basicMapper.Map(f.Requester))
                .ToList() ?? new(),
            Following = source.Item1.FollowingUsers?
                .Where(f => f is { DoesFollow: true, IsAccepted: true })
                .Select(f => basicMapper.Map(f.Follows))
                .ToList() ?? new()
        };

        return dto;
    }
}