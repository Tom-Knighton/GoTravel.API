using GoTravel.API.Domain.Models.Database;
using GoTravel.API.Domain.Models.DTOs;
using GoTravel.API.Domain.Services.Mappers;

namespace GoTravel.API.Services.Services.Mappers;

public class LineModeMapper: IMapper<GLLineMode, LineModeDto>
{
    private IMapper<GLFlag, string> _flagMapper;

    public LineModeMapper(IMapper<GLFlag, string> flagMap)
    {
        _flagMapper = flagMap;
    }
    
    
    public LineModeDto Map(GLLineMode source)
    {
        var dest = new LineModeDto
        {
            LineModeName = source.LineModeName,
        };

        var lines = source.Lines?.Select(l => new LineDto
        {
            LineName = l.LineName,
            LinePrimaryColour = l.BrandingColour
        }) ?? new List<LineDto>();

        dest.Lines = lines.ToList();
        
        dest.Branding = new LineModeBrandingDto
        {
            LineModeLogoUrl = source.LogoUrl,
            LineModePrimaryColour = source.PrimaryColour,
            LineModeBackgroundColour = source.BrandingColour,
            LineModeSecondaryColour = source.SecondaryColour
        };

        if (source.PrimaryArea is not null)
        {
            dest.PrimaryAreaName = source.PrimaryArea.AreaName;
        }
        else
        {
            dest.PrimaryAreaName = "UK";
        }

        dest.Flags = source.Flags?.Select(f => _flagMapper.Map(f)).ToList() ?? new List<string>();

        return dest;
    }
}