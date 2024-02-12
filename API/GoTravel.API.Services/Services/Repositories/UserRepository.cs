using GoTravel.API.Domain.Data;
using GoTravel.API.Domain.Models.Database;
using GoTravel.API.Domain.Services.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GoTravel.API.Services.Services.Repositories;

public class UserRepository: IUserRepository
{
    private readonly GoTravelContext _context;

    public UserRepository(GoTravelContext context)
    {
        _context = context;
    }
    
    public async Task<GTUserDetails?> GetUserByIdAsync(string id, CancellationToken ct = default)
    {
        return await _context
            .Users
            .FirstOrDefaultAsync(u => u.UserId == id, cancellationToken: ct);
    }

    public async Task<GTUserDetails?> GetUserByAnIdentifierAsync(string identifier, CancellationToken ct = default)
    {
        return await _context
            .Users
            .FirstOrDefaultAsync(u => u.UserName == identifier || u.UserId == identifier, cancellationToken: ct);
    }

    public async Task SaveUser(GTUserDetails userDetails, CancellationToken ct = default)
    {
        if (_context.Users.Any(u => u.UserId == userDetails.UserId))
        {
            _context.Users.Update(userDetails);
        }
        else
        {
            _context.Users.Add(userDetails);
        }

        await _context.SaveChangesAsync(ct);
    }
}