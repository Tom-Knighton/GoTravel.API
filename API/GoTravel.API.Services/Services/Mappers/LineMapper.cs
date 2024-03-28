using GoTravel.API.Domain.Models.Database;
using GoTravel.API.Domain.Models.DTOs;
using GoTravel.API.Domain.Services.Mappers;

namespace GoTravel.API.Services.Services.Mappers;

public class LineMapper: IMapper<GTLine, LineDto>
{
    public LineDto Map(GTLine source)
    {
        return new LineDto
        {
            LineId = source.LineId,
            LineName = source.LineName,
            LinePrimaryColour = source.BrandingColour
        };
    }
}