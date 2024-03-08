using GoTravel.API.Domain.Models.Database;

namespace GoTravel.API.Domain.Services.Repositories;

public interface ICrowdsourceRepository
{
    Task<IEnumerable<GTCrowdsourceInfo>> GetCrowdsourcesAndVotesForEntity(string entityId, CancellationToken ct = default);
    Task SaveCrowdsource(GTCrowdsourceInfo info, CancellationToken ct = default);
}