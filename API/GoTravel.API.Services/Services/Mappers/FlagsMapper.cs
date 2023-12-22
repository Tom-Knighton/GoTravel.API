using GoTravel.API.Domain.Models.Database;
using GoTravel.API.Domain.Services.Mappers;

namespace GoTravel.API.Services.Services.Mappers;

public class FlagsMapper: IMapper<GLFlag, string>
{
    public string Map(GLFlag source)
    {
        return source.Flag;
    }
}