using GoTravel.API.Domain.Models.DTOs.Commands;

namespace GoTravel.API.Domain.Services;

public interface ITripService
{
    public Task SaveUserTrip(SaveUserTripCommand command, string userId, CancellationToken ct = default);
}