using GoTravel.API.Domain.Models.DTOs;
using GoTravel.API.Domain.Models.DTOs.Commands;

namespace GoTravel.API.Domain.Services;

public interface ITripService
{
    public Task<UserSavedJourneyDto?> SaveUserTrip(SaveUserTripCommand command, string userId, CancellationToken ct = default);
    public Task<ICollection<UserSavedJourneyDto>> GetTripsForUser(string userId, int results, int startFrom, CancellationToken ct = default);
}