using System.ComponentModel.DataAnnotations;
using GoTravel.API.Domain.Models.Database;

namespace GoTravel.API.Domain.Models.DTOs;

public class UpdateUserDetailsCommand
{
    public string? Username { get; set; }
    
    [EnumDataType(typeof(GTUserFollowerAcceptLevel))]
    public GTUserFollowerAcceptLevel? FollowAcceptType { get; set; }
}