using GoTravel.API.Domain.Models.Lib;

namespace GoTravel.API.Domain.Services;

public interface IPointsService
{
    Task AddPointsToUser(string userIdentifier, int points, string reason, AddPointsReasonType type, CancellationToken ct = default);
}