﻿// <auto-generated />
using GoLondon.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GoLondon.API.Migrations
{
    [DbContext(typeof(GoLondonContext))]
    partial class GoLondonContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.HasPostgresExtension(modelBuilder, "postgis");
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("GoLondon.API.Domain.Models.Database.GLLine", b =>
                {
                    b.Property<string>("LineId")
                        .HasColumnType("text");

                    b.Property<bool>("IsEnabled")
                        .HasColumnType("boolean");

                    b.Property<string>("LineModeId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("LineName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("LineId");

                    b.HasIndex("LineModeId");

                    b.ToTable("Line", (string)null);
                });

            modelBuilder.Entity("GoLondon.API.Domain.Models.Database.GLLineMode", b =>
                {
                    b.Property<string>("LineModeName")
                        .HasColumnType("text");

                    b.Property<bool>("IsEnabled")
                        .HasColumnType("boolean");

                    b.HasKey("LineModeName");

                    b.ToTable("LineMode", (string)null);
                });

            modelBuilder.Entity("GoLondon.API.Domain.Models.Database.GLStopPoint", b =>
                {
                    b.Property<string>("StopPointId")
                        .HasColumnType("text");

                    b.Property<string>("BikesAvailable")
                        .HasColumnType("text");

                    b.Property<string>("BusStopIndicator")
                        .HasColumnType("text");

                    b.Property<string>("BusStopLetter")
                        .HasColumnType("text");

                    b.Property<string>("BusStopSMSCode")
                        .HasColumnType("text");

                    b.Property<string>("EBikesAvailable")
                        .HasColumnType("text");

                    b.Property<Point>("StopPointCoordinate")
                        .IsRequired()
                        .HasColumnType("geometry");

                    b.Property<string>("StopPointHub")
                        .HasColumnType("text");

                    b.Property<string>("StopPointName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("StopPointParentId")
                        .HasColumnType("text");

                    b.Property<string>("StopPointType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("StopPointId");

                    b.ToTable("StopPoint", (string)null);
                });

            modelBuilder.Entity("GoLondon.API.Domain.Models.Database.GLStopPointLine", b =>
                {
                    b.Property<string>("StopPointId")
                        .HasColumnType("text");

                    b.Property<string>("LineId")
                        .HasColumnType("text");

                    b.Property<bool>("IsEnabled")
                        .HasColumnType("boolean");

                    b.HasKey("StopPointId", "LineId");

                    b.HasIndex("LineId");

                    b.ToTable("StopPointLine", (string)null);
                });

            modelBuilder.Entity("GoLondon.API.Domain.Models.Database.GLLine", b =>
                {
                    b.HasOne("GoLondon.API.Domain.Models.Database.GLLineMode", "LineMode")
                        .WithMany("Lines")
                        .HasForeignKey("LineModeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("LineMode");
                });

            modelBuilder.Entity("GoLondon.API.Domain.Models.Database.GLStopPointLine", b =>
                {
                    b.HasOne("GoLondon.API.Domain.Models.Database.GLLine", "Line")
                        .WithMany("StopPointLines")
                        .HasForeignKey("LineId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GoLondon.API.Domain.Models.Database.GLStopPoint", "StopPoint")
                        .WithMany("StopPointLines")
                        .HasForeignKey("StopPointId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Line");

                    b.Navigation("StopPoint");
                });

            modelBuilder.Entity("GoLondon.API.Domain.Models.Database.GLLine", b =>
                {
                    b.Navigation("StopPointLines");
                });

            modelBuilder.Entity("GoLondon.API.Domain.Models.Database.GLLineMode", b =>
                {
                    b.Navigation("Lines");
                });

            modelBuilder.Entity("GoLondon.API.Domain.Models.Database.GLStopPoint", b =>
                {
                    b.Navigation("StopPointLines");
                });
#pragma warning restore 612, 618
        }
    }
}
