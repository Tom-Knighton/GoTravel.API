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
            ScoreboardUsers = source.Users?.Select(u => new ScoreboardUserDto
            {
                Points = u.Points,
                User = userMapper.Map(u.User)

            }).ToList() ?? new()
        };

        return dto;
    }
}