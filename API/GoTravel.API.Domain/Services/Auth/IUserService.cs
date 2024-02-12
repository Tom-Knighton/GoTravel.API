using GoTravel.API.Domain.Models.DTOs;
using GoTravel.API.Domain.Models.Lib;

namespace GoTravel.API.Domain.Services.Auth;

public interface IUserService
{ 
    /// <summary>
    /// Returns information for a specified user as a DTO.
    /// </summary>
    public Task<UserDto?> GetUserInfoByIdOrUsername(string identifier, CancellationToken ct = default);
    
    /// <summary>
    /// Returns detailed information for the current user as a DTO
    /// </summary>
    /// <remarks>Request must have a valid user claim</remarks>
    public Task<CurrentUserDto> GetCurrentUserInfo(CancellationToken ct = default);

    /// <summary>
    /// Creates a new user from existing Auth provider details
    /// </summary>
    public Task CreateUser(AuthUserSignup dto, CancellationToken ct = default);
}