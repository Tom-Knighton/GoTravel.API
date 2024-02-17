using System.Security.Claims;
using GoTravel.API.Domain.Exceptions;
using GoTravel.API.Domain.Models.Database;
using GoTravel.API.Domain.Models.DTOs;
using GoTravel.API.Domain.Models.Lib;
using GoTravel.API.Domain.Services.Auth;
using GoTravel.API.Domain.Services.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Minio;
using Minio.DataModel.Args;
using Minio.DataModel.Response;

namespace GoTravel.API.Services.Services.Auth;

public class UserService: IUserService
{
    private readonly IAuthService _authService;
    private readonly IUserRepository _userRepo;
    private readonly IHttpContextAccessor _context;
    private readonly IMinioClient _minio;
    
    private const string DbConnectionPrefix = "auth0|";
    private const string ProfilePicBucket = "gotravel";
    
    public UserService(IAuthService authService, IUserRepository repo, IHttpContextAccessor context, IMinioClient minio)
    {
        _authService = authService;
        _userRepo = repo;
        _context = context;
        _minio = minio;
    }


    public async Task ThrowIfUserOperatingOnOtherUser(string identifyingAgainst, CancellationToken ct = default)
    {
        var dbUser = await _userRepo.GetUserByAnIdentifierAsync(identifyingAgainst, ct);
        if (dbUser is null)
        {
            throw new UserNotFoundException(identifyingAgainst);
        }

        var currentId = _context.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (dbUser.UserId != currentId)
        {
            throw new WrongUserException(identifyingAgainst);
        }
    }

    public async Task<UserDto?> GetUserInfoByIdOrUsername(string identifier, CancellationToken ct = default)
    {
        var details = await _userRepo.GetUserByAnIdentifierAsync(identifier, ct);
        if (details is null)
        {
            throw new UserNotFoundException(identifier);
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
            throw new UserNotFoundException("No userinfo from token");
        }
        
        var details = await _userRepo.GetUserByIdAsync(userInfo.sub, ct);
        if (details is null)
        {
            throw new UserNotFoundException(userInfo.sub);
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

    public async Task<bool> UpdateUserDetails(string username, UpdateUserDetailsDto dto, CancellationToken ct = default)
    {
        var user = await _userRepo.GetUserByAnIdentifierAsync(username, ct);
        if (user is null)
        {
            throw new UserNotFoundException(username);
        }

        if (user.UserId.StartsWith(DbConnectionPrefix))
        {
            var authSuccess = await _authService.UpdateUsername(dto.username, user.UserId, ct);

            if (!authSuccess)
            {
                return false;
            }
        }

        try
        {
            user.UserName = dto.username;
            await _userRepo.SaveUser(user, ct);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateProfilePictureUrl(string username, IFormFile picture, CancellationToken ct = default)
    {
        var user = await _userRepo.GetUserByAnIdentifierAsync(username, ct);
        if (user is null)
        {
            throw new UserNotFoundException(username);
        }

        var stream = picture.OpenReadStream();

        var guid = Guid.NewGuid().ToString("N");
        var args = new PutObjectArgs()
            .WithBucket(ProfilePicBucket)
            .WithObject($"Users/{user.UserId}/{guid}{Path.GetExtension(picture.FileName)}")
            .WithStreamData(stream)
            .WithObjectSize(stream.Length)
            .WithContentType(GetMimeType(picture.FileName));

        try
        {
            var oldUrl = user.UserProfilePicUrl;
            await _minio.PutObjectAsync(args, ct);

            var url = $"https://{_minio.Config.BaseUrl}/{ProfilePicBucket}/Users/{user.UserId}/{guid}{Path.GetExtension(picture.FileName)}";

            user.UserProfilePicUrl = url;
            await _userRepo.SaveUser(user, ct);

            var success = true;
            
            if (user.UserId.StartsWith(DbConnectionPrefix))
            {
               success = await _authService.UpdateProfilePictureUrl(url, user.UserId, ct);
            }

            if (success)
            {
                try
                {
                    var removeArgs = new RemoveObjectArgs()
                        .WithBucket(ProfilePicBucket)
                        .WithObject(oldUrl.Replace("https://cdn.tomk.online/gotravel/", ""));
                    await _minio.RemoveObjectAsync(removeArgs, ct);
                } catch {}
            }

            return success;
        }
        catch
        {
            //TODO: Log
            return false;
        }
    }
    
    private string GetMimeType(string fileName)
    {
        var provider = new FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(fileName, out var contentType))
        {
            contentType = "application/octet-stream";
        }
        return contentType;            
    }
}