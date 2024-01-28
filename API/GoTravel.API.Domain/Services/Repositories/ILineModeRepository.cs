using GoTravel.API.Domain.Models.Database;
using GoTravel.API.Domain.Models.DTOs;

namespace GoTravel.API.Domain.Services.Repositories;

public interface ILineModeRepository
{
    Task<ICollection<GLLineMode>> GetLineModes(bool includeDisabled = false, CancellationToken ct = default);
    Task<GLLineMode?> GetLineMode(string id, bool includeDisabled = false, CancellationToken ct = default);
    
    
    /// <summary>
    /// Returns line modes for specified line ids, will only return the lines with ids in lineIds
    /// </summary>
    Task<ICollection<GLLineMode>> GetLineModesByLineIds(ICollection<string> lineIds, bool includeDisabled = false, CancellationToken ct = default);

    Task<GLLineMode> Update(GLLineMode mode, CancellationToken ct = default);
}