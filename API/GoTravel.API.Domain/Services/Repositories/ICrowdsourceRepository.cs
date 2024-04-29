using GoTravel.API.Domain.Models.Database;

namespace GoTravel.API.Domain.Services.Repositories;

public interface ICrowdsourceRepository
{
    Task<IEnumerable<GTCrowdsourceInfo>> GetCrowdsourcesAndVotesForEntity(string entityId, bool includeReports = false, CancellationToken ct = default);
    Task SaveCrowdsource(GTCrowdsourceInfo info, CancellationToken ct = default);

    Task<GTCrowdsourceInfo?> GetCrowdsource(string id, CancellationToken ct = default);
    Task<GTCrowdsourceVotes?> GetVote(string crowdsourceId, string userId, CancellationToken ct = default);
    Task<bool> DeleteVote(string crowdsourceId, string userId, CancellationToken ct = default);
    Task<bool> SaveVote(GTCrowdsourceVotes vote, CancellationToken ct = default);
    Task<bool> SaveReport(GTCrowdsourceReport report, CancellationToken ct = default);
}