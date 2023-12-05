using GoTravel.API.Domain.Models.Database;

namespace GoTravel.API.Domain.Services.Repositories;

public interface ILineModeRepository
{
    Task<ICollection<GLLineMode>> GetLineModes(CancellationToken ct = default);
}