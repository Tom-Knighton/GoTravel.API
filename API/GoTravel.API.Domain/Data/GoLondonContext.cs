using System.Reflection;
using GoTravel.API.Domain.Models.Database;
using GoTravel.Standard.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json.Converters;

namespace GoTravel.API.Domain.Data;

public class GoTravelContext: DbContext
{
    public GoTravelContext() {}
    public GoTravelContext(DbContextOptions<GoTravelContext> options): base(options) {}
    
    public virtual DbSet<GTStopPoint> StopPoints { get; set; }
    public virtual DbSet<GLLine> Lines { get; set; }
    public virtual DbSet<GLLineMode> LineModes { get; set; }
    public virtual DbSet<GLStopPointLine> StopPointLines { get; set; }
    public virtual DbSet<GTArea> Areas { get; set; }
    public virtual DbSet<GTStopPointInfoKey> StopPointInfoKeys { get; set; }
    public virtual DbSet<GTStopPointInfoValue> StopPointInfoValues { get; set; }
    
    public virtual DbSet<GTUserDetails> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<GLFlag>(e =>
        {
            e.ToTable("Flags");
            e.Property(f => f.GLFlagId).ValueGeneratedOnAdd();
            e.HasKey(f => f.GLFlagId);
        });
        
        modelBuilder.Entity<GTStopPoint>(e =>
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
            e.HasKey(lm => lm.LineModeId);

            e.HasOne(lm => lm.PrimaryArea)
                .WithMany(a => a.LineModes)
                .HasForeignKey(lm => lm.AreaId);

            e.HasMany(lm => lm.Flags)
                .WithMany()
                .UsingEntity(join => join.ToTable("Flags_LineModes"));
        });

        modelBuilder.Entity<GTArea>(e =>
        {
            e.ToTable("Area");
            e.Property(a => a.AreaId).ValueGeneratedOnAdd();
            e.HasKey(a => a.AreaId);
        });

        modelBuilder.Entity<GTStopPointInfoKey>(e =>
        {
            e.ToTable("StopPointInfoKeys");
            e.HasKey(k => k.InfoKey);
            e.Property(k => k.InfoKey).HasConversion<EnumToStringConverter<StopPointInfoKey>>();
        });

        modelBuilder.Entity<GTStopPointInfoValue>(e =>
        {
            e.ToTable("StopPointInfo");
            e.HasKey(i => new { i.StopPointId, i.KeyId });
            e.HasOne(i => i.Key)
                .WithMany()
                .HasForeignKey(i => i.KeyId);
            e.HasOne(i => i.StopPoint)
                .WithMany()
                .HasForeignKey(i => i.StopPointId);
        });

        modelBuilder.Entity<GTUserDetails>(e =>
        {
            e.ToTable("User");
            e.HasKey(u => u.UserId);
            e.HasIndex(u => u.UserName).IsUnique();
        });
    }
}