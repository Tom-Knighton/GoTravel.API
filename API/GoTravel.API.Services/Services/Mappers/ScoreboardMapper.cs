using GoTravel.API.Domain.Models.Database;
using GoTravel.API.Domain.Models.DTOs;
using GoTravel.API.Domain.Services.Mappers;

namespace GoTravel.API.Services.Services.Mappers;

public class ScoreboardMapper(IMapper<GTUserDetails, UserDto> userMapper): IMapper<GTScoreboard, ScoreboardDto>
{
    public ScoreboardDto Map(GTScoreboard source)
    {
        var dto = new ScoreboardDto
        {
            ScoreboardName = source.ScoreboardName,
            ScoreboardDescription = source.ScoreboardDescription,
            ScoreboardLogoUrl = source.ScoreboardIconUrl,
            ScoreboardUsers = new List<ScoreboardUserDto>()
        };

        var ordered = source.Users.OrderByDescending(u => u.Points).ToList();
        for (var i = 1; i <= ordered.Count; i++)
        {
            var user = ordered.ElementAt(i - 1);
            dto.ScoreboardUsers.Add(new ScoreboardUserDto
            {
                Points = user.Points,
                User = userMapper.Map(user.User),
                Rank = i
            });
        }
        
        return dto;
    }
}