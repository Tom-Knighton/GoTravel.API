using GoTravel.API.Domain.Models.Database;
using GoTravel.Standard.Models;
using Microsoft.EntityFrameworkCore;

namespace GoTravel.API.Domain.Data;

public class GoTravelSeeder
{
    private readonly GoTravelContext _context;

    public GoTravelSeeder(GoTravelContext context)
    {
        _context = context;
    }

    public void Seed()
    {
        SeedInfoKeys();
    }

    private void SeedInfoKeys()
    {
        foreach (var value in Enum.GetValues<StopPointInfoKey>())
        {
            var existingKey = _context.StopPointInfoKeys.FirstOrDefault(k => k.InfoKey == value);
            if (existingKey is null)
            {
                _context.StopPointInfoKeys.Add(new GTStopPointInfoKey
                {
                    InfoKey = value
                });
            }
        }

        _context.SaveChanges();
    }
}