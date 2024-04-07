using GoTravel.API.Domain.Models.Database;
using GoTravel.API.Domain.Models.DTOs;
using GoTravel.API.Domain.Models.Lib;
using Microsoft.AspNetCore.Http;

namespace GoTravel.API.Domain.Services.Auth;

public interface IUserService
{
    /// <summary>
    /// Throws an exception if the CurrentUser is not the same user as the operation is being completed against
    /// </summary>
    /// <param name="identifyingAgainst">A username or id of a user to operate on</param>
    public Task ThrowIfUserOperatingOnOtherUser(string identifyingAgainst, CancellationToken ct = default);
    
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
    
    /// <summary>
    /// Updates a user's basic details with the ones provided
    /// </summary>
    public Task<bool> UpdateUserDetails(string username, UpdateUserDetailsCommand command, CancellationToken ct = default);

    /// <summary>
    /// Uploads a profile picture to a CDN and updates the user's details to point to the uploaded url
    /// </summary>
    public Task<bool> UpdateProfilePictureUrl(string username, IFormFile picture, CancellationToken ct = default);

    /// <summary>
    /// Returns a list of UserDtos IGNORING the current user, based on a case-insensitive query
    /// </summary>
    Task<ICollection<UserDto>> SearchUsers(string query, int maxResults, CancellationToken ct = default);

    /// <summary>
    /// 
    /// </summary>
    Task AddSubtitle(string userId, GTUserSubtitle subtitle, CancellationToken ct = default);
}