namespace GoTravel.API.Domain.Services;

public interface IPointsService
{
    Task AddPointsToUser(string userIdentifier, int points, string reason, CancellationToken ct = default);
}