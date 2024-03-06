using System.Numerics.Tensors;
using GoTravel.API.Domain.Models.Database;
using GoTravel.API.Domain.Models.DTOs;
using GoTravel.API.Domain.Models.DTOs.Commands;
using GoTravel.API.Domain.Services;
using GoTravel.API.Domain.Services.Mappers;
using GoTravel.API.Domain.Services.Repositories;
namespace GoTravel.API.Services.Services;

public class CrowdsourceService: ICrowdsourceService
{
    private readonly ICrowdsourceRepository _repo;
    private readonly IMapper<GTCrowdsourceInfo, CrowdsourceInfoDto> _mapper;
    private const double SemanticSimilarityThreshold = 0.7;

    public CrowdsourceService(ICrowdsourceRepository repo, IMapper<GTCrowdsourceInfo, CrowdsourceInfoDto> mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public Task SubmitCrowdsourceInfo(string userId, AddCrowdsourceCommand command, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<CrowdsourceInfoDto>> GetCrowdsourceInfoForEntities(string entityId, CancellationToken ct = default)
    {
        var crowdsourceUploads = await _repo.GetCrowdsourcesAndVotesForEntity(entityId, ct);
        var uniquePostGroups = GroupBySemanticSimilarity(crowdsourceUploads);

        var info = new List<GTCrowdsourceInfo>();
        
        foreach (var group in uniquePostGroups)
        {
            var best = group.MaxBy(g => g.Votes);
            if (best is null) continue;

            best.IsClosed = group.Any(c => c.IsClosed);
            best.IsDelayed = group.Any(c => c.IsDelayed);
            
            info.Add(best);
        }

        var dtos = info.Select(i => _mapper.Map(i));

        return dtos;
    }


    private IEnumerable<IEnumerable<GTCrowdsourceInfo>> GroupBySemanticSimilarity(IEnumerable<GTCrowdsourceInfo> crowdsourceInfos)
    {
        var groups = new List<List<GTCrowdsourceInfo>>();

        foreach (var info in crowdsourceInfos)
        {
            var isGrouped = false;
            foreach (var group in groups.Where(group => IsSemanticallySimilarToGroup(info.Embeddings, group)))
            {
                group.Add(info);
                isGrouped = true;
                break;
            }

            if (!isGrouped)
            {
                groups.Add([info]);
            }
        }

        return groups;
    }

    private static bool IsSemanticallySimilarToGroup(float[] embedding, IEnumerable<GTCrowdsourceInfo> group)
    {
        return group.Any(member => CosineSimilarity(embedding, member.Embeddings) > SemanticSimilarityThreshold);
    }

    private static double CosineSimilarity(float[] a, float[] b)
    {
        if (a.Length == 0 || b.Length == 0)
        {
            return 0;
        }
        
        return TensorPrimitives.CosineSimilarity(a, b);
    }
}