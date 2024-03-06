using GoTravel.API.Domain.Models.Database;

namespace GoTravel.API.Domain.Services.Repositories;

public interface ICrowdsourceRepository
{
    Task<ICollection<GTCrowdsourceInfo>> GetCrowdsourcesAndVotesForEntity(string entityId, CancellationToken ct = default);
}