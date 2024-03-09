using System.Numerics.Tensors;
using GoTravel.API.Domain.Exceptions;
using GoTravel.API.Domain.Models.Database;
using GoTravel.API.Domain.Models.DTOs;
using GoTravel.API.Domain.Models.DTOs.Commands;
using GoTravel.API.Domain.Services;
using GoTravel.API.Domain.Services.Mappers;
using GoTravel.API.Domain.Services.Repositories;
using MassTransit.Configuration;
using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Chat;

namespace GoTravel.API.Services.Services;

public class CrowdsourceService: ICrowdsourceService
{
    private readonly ICrowdsourceRepository _repo;
    private readonly IMapper<GTCrowdsourceInfo, CrowdsourceInfoDto> _mapper;
    private const double SemanticSimilarityThreshold = 0.6;
    private readonly OpenAIClient _ai;
    private const string ChatModel = "gpt-3.5-turbo";
    private const string EmbeddingModel = "text-embedding-3-small";

    private const string ModerationSystem = "You are a moderation system. You are only capable of responding 'y' or 'n'. Respond 'y' if the submitted text is relevant, useful and not harmful in the context of crowdsourced info for a public transport app. It should pass if the text is useful and helpful for other users. Respond 'n' if it's not relevant, or harmful, or contains swear words/violence. It can pass even if there's personal language like 'my'.";

    public CrowdsourceService(ICrowdsourceRepository repo, IMapper<GTCrowdsourceInfo, CrowdsourceInfoDto> mapper, IConfiguration config)
    {
        _repo = repo;
        _mapper = mapper;

        _ai = new OpenAIClient(new OpenAIAuthentication(config.GetSection("OpenAI").GetValue<string>("Key")));
    }

    public async Task SubmitCrowdsourceInfo(string userId, string entityId, AddCrowdsourceCommand command, CancellationToken ct = default)
    {
        var newSubmission = new GTCrowdsourceInfo
        {
            EntityId = entityId,
            IsDelayed = command.IsDelayed,
            IsClosed = command.IsClosed,
            ExpectedEnd = command.ExpectedEnd.ToUniversalTime(),
            SubmittedAt = command.StartsAt.ToUniversalTime(),
            FreeText = command.FreeText,
            UUID = Guid.NewGuid().ToString("N"),
            SubmittedById = userId,
            NeedsReview = false,
        };
        
        if (!string.IsNullOrWhiteSpace(command.FreeText))
        {
            var moderationResult = await _ai.ChatEndpoint.GetCompletionAsync(new ChatRequest(new List<Message>
            {
                new (Role.System, ModerationSystem),
                new (Role.User, command.FreeText)
            }, model: ChatModel), ct);

            if (moderationResult?.FirstChoice.Message.Content.ToString() != "y")
            {
                throw new BadModerationException(command.FreeText, userId, entityId);
            }

            var embeddingsResponse = await _ai.EmbeddingsEndpoint.CreateEmbeddingAsync(command.FreeText, EmbeddingModel, cancellationToken: ct);
            newSubmission.Embeddings = embeddingsResponse.Data
                .FirstOrDefault()?
                .Embedding
                .Select(e => (float)e)
                .ToArray() ?? [];
        }
        
        await _repo.SaveCrowdsource(newSubmission, ct);
    }

    public async Task<IEnumerable<CrowdsourceInfoDto>> GetCrowdsourceInfoForEntities(string entityId, CancellationToken ct = default)
    {
        var crowdsourceUploads = await _repo.GetCrowdsourcesAndVotesForEntity(entityId, ct);
        var uniquePostGroups = GroupBySemanticSimilarity(crowdsourceUploads);

        var info = new List<GTCrowdsourceInfo>();
        
        foreach (var group in uniquePostGroups)
        {
            var best = group.MaxBy(g => g.Votes.Count);
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
        
        var similarity = TensorPrimitives.CosineSimilarity(a, b);
        return similarity;
    }
}