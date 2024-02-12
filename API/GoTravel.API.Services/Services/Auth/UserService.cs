using GoTravel.API.Domain.Models.Database;
using GoTravel.API.Domain.Models.DTOs;
using GoTravel.API.Domain.Models.Lib;
using GoTravel.API.Domain.Services.Auth;
using GoTravel.API.Domain.Services.Repositories;

namespace GoTravel.API.Services.Services.Auth;

public class UserService: IUserService
{
    private readonly IAuthService _authService;
    private readonly IUserRepository _userRepo;
    
    public UserService(IAuthService authService, IUserRepository repo)
    {
        _authService = authService;
        _userRepo = repo;
    }
    
    
    public async Task<UserDto?> GetUserInfoByIdOrUsername(string identifier, CancellationToken ct = default)
    {
        var details = await _userRepo.GetUserByAnIdentifierAsync(identifier, ct);
        if (details is null)
        {
            throw new Exception();
        }
        
        return new UserDto
        { 
            UserName = details.UserName,
            UserPictureUrl = details.UserProfilePicUrl
        };
    }

    public async Task<CurrentUserDto> GetCurrentUserInfo(CancellationToken ct = default)
    {
        var userInfo = await _authService.GetCurrentUserInfo(ct);

        if (userInfo is null)
        {
            throw new Exception();
        }
        
        var details = await _userRepo.GetUserByIdAsync(userInfo.sub, ct);
        if (details is null)
        {
            throw new Exception();
        }
        
        return new CurrentUserDto
        {
            UserId = details.UserId,
            UserName = details.UserName,
            UserEmail = userInfo.name,
            UserPictureUrl = details.UserProfilePicUrl,
            DateCreated = details.DateCreated
        };
    }

    public async Task CreateUser(AuthUserSignup dto, CancellationToken ct = default)
    {
        var userDetails = new GTUserDetails
        {
            UserId = dto.userId,
            UserProfilePicUrl = dto.userPicUrl,
            DateCreated = DateTime.Parse(dto.created).ToUniversalTime(),
            UserName = dto.userName
        };

        await _userRepo.SaveUser(userDetails, ct);
    }
}