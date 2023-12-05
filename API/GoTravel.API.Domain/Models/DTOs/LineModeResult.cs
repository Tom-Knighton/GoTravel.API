using GoTravel.API.Domain.Models.Database;

namespace GoTravel.API.Domain.Models.DTOs;

public class LineModeSearchResult
{
    public string AreaName { get; set; }
    public ICollection<LineModeDto> LineModes { get; set; }
}