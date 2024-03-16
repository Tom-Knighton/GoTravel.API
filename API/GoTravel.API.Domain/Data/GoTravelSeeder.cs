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
        SeedScoreboard();
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

    private void SeedScoreboard()
    {
        var existingGS = _context.Scoreboards.FirstOrDefault(s => s.ScoreboardName == "Most Travel");
        if (existingGS is null)
        {
            _context.Scoreboards.Add(new GTScoreboard
            {
                UUID = Guid.NewGuid().ToString("N"),
                ScoreboardName = "Most Travel",
                ActiveFrom = DateTime.UtcNow,
                ScoreboardDescription = "Compete against friends to use the most public transport!",
                JoinType = GTScoreboadJoinType.AllEnrolled,
            });
            _context.SaveChanges();
        }
    }
}