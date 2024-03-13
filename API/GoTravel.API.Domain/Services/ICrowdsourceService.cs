using GoTravel.API.Domain.Models.DTOs;
using GoTravel.API.Domain.Models.DTOs.Commands;

namespace GoTravel.API.Domain.Services;

public interface ICrowdsourceService
{
    Task SubmitCrowdsourceInfo(string userId, string entityId, AddCrowdsourceCommand command, CancellationToken ct = default);
    Task<IEnumerable<CrowdsourceInfoDto>> GetCrowdsourceInfoForEntities(string entityId, CancellationToken ct = default);
    
    Task<bool> VoteOnCrowdsource(string crowdsourceId, string userId, CrowdsourceVoteStatus voteType, CancellationToken ct = default);
    Task<bool> ReportCrowdsource(string crowdsourceId, string userId, ReportCrowdsourceCommand command, CancellationToken ct = default);
}