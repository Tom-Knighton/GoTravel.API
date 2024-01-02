using GoTravel.API.Domain.Models.Database;

namespace GoTravel.API.Domain.Services.Repositories;

public interface ILineModeRepository
{
    Task<ICollection<GLLineMode>> GetLineModes(bool includeDisabled = false, CancellationToken ct = default);
    Task<GLLineMode?> GetLineMode(string id, bool includeDisabled = false, CancellationToken ct = default);

    Task<GLLineMode> Update(GLLineMode mode, CancellationToken ct = default);
}