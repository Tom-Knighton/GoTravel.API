using System.ComponentModel.DataAnnotations;

namespace GoTravel.API.Domain.Models.Database;

public enum GTCrowdsourceVoteType
{
    upvote,
    downvote
}

public class GTCrowdsourceVotes
{
    public string CrowdsourceId { get; set; }
    public string UserId { get; set; }
    
    [EnumDataType(typeof(GTCrowdsourceVoteType))]
    public GTCrowdsourceVoteType VoteType { get; set; }
    
    public virtual GTCrowdsourceInfo Crowdsource { get; set; }
}