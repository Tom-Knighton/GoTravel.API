using GoTravel.API.Domain.Exceptions;
using GoTravel.API.Domain.Models.Database;
using GoTravel.API.Domain.Services;
using GoTravel.API.Domain.Services.Repositories;
using Microsoft.Extensions.Logging;

namespace GoTravel.API.Services.Services;

public class PointsService : IPointsService
{
    private readonly IUserRepository _userRepo;
    private readonly ILogger<PointsService> _logger;
    private readonly TimeProvider _times;
    
    public PointsService(IUserRepository repo, ILogger<PointsService> log, TimeProvider time)
    {
        _userRepo = repo;
        _logger = log;
        _times = time;
    }
    
    public async Task AddPointsToUser(string userIdentifier, int points, string reason, CancellationToken ct = default)
    {
        var user = await _userRepo.GetUserByAnIdentifierAsync(userIdentifier, ct);
        if (user is null)
        {
            throw new UserNotFoundException(userIdentifier);
        }

        if (points > 20)
        {
            _logger.LogWarning("WARN > 20 points being added to user, {Id} for {Reason}", userIdentifier, reason);
        }

        var audit = new GTUserPointsAudit
        {
            UserId = user.UserId,
            PointsAtAdd = user.UserPoints,
            UpdatedAt = _times.GetUtcNow().UtcDateTime,
            PointsAdded = points,
            Reason = reason
        };

        user.UserPoints += points;

        await _userRepo.AddUserAudit(audit, ct);
        await _userRepo.SaveUser(user, ct);
    }
}