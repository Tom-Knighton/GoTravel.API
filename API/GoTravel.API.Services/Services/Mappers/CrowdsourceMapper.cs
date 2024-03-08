using GoTravel.API.Domain.Extensions;
using GoTravel.API.Domain.Models.Database;
using GoTravel.API.Domain.Models.DTOs;
using GoTravel.API.Domain.Services.Mappers;
using Microsoft.AspNetCore.Http;

namespace GoTravel.API.Services.Services.Mappers;

public class CrowdsourceMapper: IMapper<GTCrowdsourceInfo, CrowdsourceInfoDto>
{
    private readonly HttpContext? _context;
    private readonly IMapper<GTUserDetails, UserDto> _userMap;
    
    public CrowdsourceMapper(IHttpContextAccessor context, IMapper<GTUserDetails, UserDto> userMap)
    {
        _context = context.HttpContext;
        _userMap = userMap;
    }
    
    public CrowdsourceInfoDto Map(GTCrowdsourceInfo source)
    {
        var dto = new CrowdsourceInfoDto
        {
            CrowdsourceId = source.UUID,
            SubmittedBy = _userMap.Map(source.SubmittedBy),
            IsDelayed = source.IsDelayed,
            ExpectedEnd = source.ExpectedEnd,
            IsClosed = source.IsClosed,
            Started = source.SubmittedAt,
            Text = source.FreeText,
            IsFlagged = source.NeedsReview,
            Score = source.Votes.Count(v => v.VoteType == GTCrowdsourceVoteType.upvote) - source.Votes.Count(v => v.VoteType == GTCrowdsourceVoteType.downvote),
            CurrentUserVoteStatus = CrowdsourceVoteStatus.NoVote,
        };

        if (_context is not null && !string.IsNullOrWhiteSpace(_context.User?.CurrentUserId()))
        {
            var voteForUser = source.Votes.FirstOrDefault(v => v.UserId == _context.User.CurrentUserId());
            if (voteForUser is not null)
            {
                dto.CurrentUserVoteStatus = voteForUser.VoteType switch
                {
                    GTCrowdsourceVoteType.upvote => CrowdsourceVoteStatus.Upvoted,
                    GTCrowdsourceVoteType.downvote => CrowdsourceVoteStatus.Downvoted,
                    _ => CrowdsourceVoteStatus.NoVote
                };
            }
        }
        
        return dto;
    }
}