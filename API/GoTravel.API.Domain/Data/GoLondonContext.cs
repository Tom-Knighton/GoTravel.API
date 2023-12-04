using System.Reflection;
using GoTravel.API.Domain.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace GoTravel.API.Domain.Data;

public class GoTravelContext: DbContext
{
    public GoTravelContext() {}
    public GoTravelContext(DbContextOptions<GoTravelContext> options): base(options) {}
    
    public virtual DbSet<GLStopPoint> StopPoints { get; set; }
    public virtual DbSet<GLLine> Lines { get; set; }
    public virtual DbSet<GLLineMode> LineModes { get; set; }
    public virtual DbSet<GLStopPointLine> StopPointLines { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<GLStopPoint>(e =>
        {
            e.ToTable("StopPoint");
            e.HasKey(s => s.StopPointId);
            e.Property(s => s.StopPointType).HasConversion<string>();
            
            e.HasMany(s => s.StopPointLines)
                .WithOne(l => l.StopPoint)
                .HasForeignKey(l => l.StopPointId);

            e.HasMany(s => s.Children)
                .WithOne(s => s.Parent)
                .HasForeignKey(s => s.StopPointParentId);
        });

        modelBuilder.Entity<GLStopPointLine>(e =>
        {
            e.ToTable("StopPointLine");
            e.HasKey(l => new { l.StopPointId, l.LineId });
            e.HasOne(l => l.Line)
                .WithMany(l => l.StopPointLines)
                .HasForeignKey(l => l.LineId);
        });

        modelBuilder.Entity<GLLine>(e =>
        {
            e.ToTable("Line");
            e.HasKey(l => l.LineId);
            e.HasOne(l => l.LineMode)
                .WithMany(m => m.Lines)
                .HasForeignKey(l => l.LineModeId);
        });

        modelBuilder.Entity<GLLineMode>(e =>
        {
            e.ToTable("LineMode");
            e.HasKey(lm => lm.LineModeName);
        });
    }
}