using System.ComponentModel.DataAnnotations;

namespace GoTravel.API.Domain.Models.Database;

public enum CrowdsourceVoteType
{
    upvote,
    downvote
}

public class GTCrowdsourceVotes
{
    public string CrowdsourceId { get; set; }
    public string UserId { get; set; }
    
    [EnumDataType(typeof(CrowdsourceVoteType))]
    public CrowdsourceVoteType VoteType { get; set; }
    
    public virtual GTCrowdsourceInfo Crowdsource { get; set; }
}