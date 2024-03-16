﻿// <auto-generated />
using System;
using GoTravel.API.Domain.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GoTravel.API.Migrations
{
    [DbContext(typeof(GoTravelContext))]
    partial class GoTravelContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.HasPostgresExtension(modelBuilder, "postgis");
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("GLFlagGLLineMode", b =>
                {
                    b.Property<int>("FlagsGLFlagId")
                        .HasColumnType("integer");

                    b.Property<string>("GLLineModeLineModeId")
                        .HasColumnType("text");

                    b.HasKey("FlagsGLFlagId", "GLLineModeLineModeId");

                    b.HasIndex("GLLineModeLineModeId");

                    b.ToTable("Flags_LineModes", (string)null);
                });

            modelBuilder.Entity("GoTravel.API.Domain.Models.Database.GLFlag", b =>
                {
                    b.Property<int>("GLFlagId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("GLFlagId"));

                    b.Property<string>("Flag")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("GLFlagId");

                    b.ToTable("Flags", (string)null);
                });

            modelBuilder.Entity("GoTravel.API.Domain.Models.Database.GLLine", b =>
                {
                    b.Property<string>("LineId")
                        .HasColumnType("text");

                    b.Property<string>("BrandingColour")
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

            modelBuilder.Entity("GoTravel.API.Domain.Models.Database.GLLineMode", b =>
                {
                    b.Property<string>("LineModeId")
                        .HasColumnType("text");

                    b.Property<int?>("AreaId")
                        .HasColumnType("integer");

                    b.Property<string>("BrandingColour")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsEnabled")
                        .HasColumnType("boolean");

                    b.Property<string>("LineModeName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("LogoUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PrimaryColour")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("SecondaryColour")
                        .HasColumnType("text");

                    b.HasKey("LineModeId");

                    b.HasIndex("AreaId");

                    b.ToTable("LineMode", (string)null);
                });

            modelBuilder.Entity("GoTravel.API.Domain.Models.Database.GLStopPointLine", b =>
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

            modelBuilder.Entity("GoTravel.API.Domain.Models.Database.GTArea", b =>
                {
                    b.Property<int>("AreaId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("AreaId"));

                    b.Property<Polygon>("AreaCatchment")
                        .IsRequired()
                        .HasColumnType("geometry");

                    b.Property<string>("AreaName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("AreaId");

                    b.ToTable("Area", (string)null);
                });

            modelBuilder.Entity("GoTravel.API.Domain.Models.Database.GTCrowdsourceInfo", b =>
                {
                    b.Property<string>("UUID")
                        .HasColumnType("text");

                    b.Property<float[]>("Embeddings")
                        .IsRequired()
                        .HasColumnType("real[]");

                    b.Property<string>("EntityId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("ExpectedEnd")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("FreeText")
                        .HasColumnType("text");

                    b.Property<bool>("IsClosed")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsDelayed")
                        .HasColumnType("boolean");

                    b.Property<bool>("NeedsReview")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("SubmittedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("SubmittedById")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("UUID");

                    b.HasIndex("EntityId");

                    b.HasIndex("SubmittedById");

                    b.ToTable("Crowdsource", (string)null);
                });

            modelBuilder.Entity("GoTravel.API.Domain.Models.Database.GTCrowdsourceReport", b =>
                {
                    b.Property<string>("UUID")
                        .HasColumnType("text");

                    b.Property<string>("CrowdsourceId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("Handled")
                        .HasColumnType("boolean");

                    b.Property<string>("ReportText")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("ReportedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("ReporterId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("UUID");

                    b.HasIndex("CrowdsourceId");

                    b.HasIndex("ReporterId");

                    b.ToTable("CrowdsourceReports", (string)null);
                });

            modelBuilder.Entity("GoTravel.API.Domain.Models.Database.GTCrowdsourceVotes", b =>
                {
                    b.Property<string>("CrowdsourceId")
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<int>("VoteType")
                        .HasColumnType("integer");

                    b.HasKey("CrowdsourceId", "UserId");

                    b.ToTable("CrowdsourceVotes", (string)null);
                });

            modelBuilder.Entity("GoTravel.API.Domain.Models.Database.GTScoreboard", b =>
                {
                    b.Property<string>("UUID")
                        .HasColumnType("text");

                    b.Property<DateTime>("ActiveFrom")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("EndsAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("JoinType")
                        .HasColumnType("integer");

                    b.Property<string>("ScoreboardDescription")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ScoreboardIconUrl")
                        .HasColumnType("text");

                    b.Property<string>("ScoreboardName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("UUID");

                    b.ToTable("Scoreboards", (string)null);
                });

            modelBuilder.Entity("GoTravel.API.Domain.Models.Database.GTScoreboardUser", b =>
                {
                    b.Property<string>("ScoreboardUUID")
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<DateTime>("JoinedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Points")
                        .HasColumnType("integer");

                    b.HasKey("ScoreboardUUID", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("ScoreboardUsers", (string)null);
                });

            modelBuilder.Entity("GoTravel.API.Domain.Models.Database.GTStopPoint", b =>
                {
                    b.Property<string>("StopPointId")
                        .HasColumnType("text");

                    b.Property<int?>("BikesAvailable")
                        .HasColumnType("integer");

                    b.Property<string>("BusStopIndicator")
                        .HasColumnType("text");

                    b.Property<string>("BusStopLetter")
                        .HasColumnType("text");

                    b.Property<string>("BusStopSMSCode")
                        .HasColumnType("text");

                    b.Property<int?>("EBikesAvailable")
                        .HasColumnType("integer");

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

                    b.HasIndex("StopPointParentId");

                    b.ToTable("StopPoint", (string)null);
                });

            modelBuilder.Entity("GoTravel.API.Domain.Models.Database.GTStopPointInfoKey", b =>
                {
                    b.Property<string>("InfoKey")
                        .HasColumnType("text");

                    b.HasKey("InfoKey");

                    b.ToTable("StopPointInfoKeys", (string)null);
                });

            modelBuilder.Entity("GoTravel.API.Domain.Models.Database.GTStopPointInfoValue", b =>
                {
                    b.Property<string>("StopPointId")
                        .HasColumnType("text");

                    b.Property<string>("KeyId")
                        .HasColumnType("text");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("StopPointId", "KeyId");

                    b.HasIndex("KeyId");

                    b.ToTable("StopPointInfo", (string)null);
                });

            modelBuilder.Entity("GoTravel.API.Domain.Models.Database.GTUserDetails", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("FollowerAcceptType")
                        .HasColumnType("integer");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("UserPoints")
                        .HasColumnType("integer");

                    b.Property<string>("UserProfilePicUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("UserId");

                    b.HasIndex("UserName")
                        .IsUnique();

                    b.ToTable("User", (string)null);
                });

            modelBuilder.Entity("GoTravel.API.Domain.Models.Database.GTUserFollowings", b =>
                {
                    b.Property<string>("RequesterId")
                        .HasColumnType("text");

                    b.Property<string>("FollowsId")
                        .HasColumnType("text");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("DoesFollow")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsAccepted")
                        .HasColumnType("boolean");

                    b.HasKey("RequesterId", "FollowsId");

                    b.HasIndex("FollowsId");

                    b.ToTable("UserFollowing", (string)null);
                });

            modelBuilder.Entity("GoTravel.API.Domain.Models.Database.GTUserPointsAudit", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("PointsAdded")
                        .HasColumnType("integer");

                    b.Property<int>("PointsAtAdd")
                        .HasColumnType("integer");

                    b.Property<string>("Reason")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("UserId", "UpdatedAt");

                    b.ToTable("UserPointsAudit", (string)null);
                });

            modelBuilder.Entity("GLFlagGLLineMode", b =>
                {
                    b.HasOne("GoTravel.API.Domain.Models.Database.GLFlag", null)
                        .WithMany()
                        .HasForeignKey("FlagsGLFlagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GoTravel.API.Domain.Models.Database.GLLineMode", null)
                        .WithMany()
                        .HasForeignKey("GLLineModeLineModeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("GoTravel.API.Domain.Models.Database.GLLine", b =>
                {
                    b.HasOne("GoTravel.API.Domain.Models.Database.GLLineMode", "LineMode")
                        .WithMany("Lines")
                        .HasForeignKey("LineModeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("LineMode");
                });

            modelBuilder.Entity("GoTravel.API.Domain.Models.Database.GLLineMode", b =>
                {
                    b.HasOne("GoTravel.API.Domain.Models.Database.GTArea", "PrimaryArea")
                        .WithMany("LineModes")
                        .HasForeignKey("AreaId");

                    b.Navigation("PrimaryArea");
                });

            modelBuilder.Entity("GoTravel.API.Domain.Models.Database.GLStopPointLine", b =>
                {
                    b.HasOne("GoTravel.API.Domain.Models.Database.GLLine", "Line")
                        .WithMany("StopPointLines")
                        .HasForeignKey("LineId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GoTravel.API.Domain.Models.Database.GTStopPoint", "StopPoint")
                        .WithMany("StopPointLines")
                        .HasForeignKey("StopPointId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Line");

                    b.Navigation("StopPoint");
                });

            modelBuilder.Entity("GoTravel.API.Domain.Models.Database.GTCrowdsourceInfo", b =>
                {
                    b.HasOne("GoTravel.API.Domain.Models.Database.GTUserDetails", "SubmittedBy")
                        .WithMany()
                        .HasForeignKey("SubmittedById")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SubmittedBy");
                });

            modelBuilder.Entity("GoTravel.API.Domain.Models.Database.GTCrowdsourceReport", b =>
                {
                    b.HasOne("GoTravel.API.Domain.Models.Database.GTUserDetails", "Reporter")
                        .WithMany()
                        .HasForeignKey("ReporterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Reporter");
                });

            modelBuilder.Entity("GoTravel.API.Domain.Models.Database.GTCrowdsourceVotes", b =>
                {
                    b.HasOne("GoTravel.API.Domain.Models.Database.GTCrowdsourceInfo", "Crowdsource")
                        .WithMany("Votes")
                        .HasForeignKey("CrowdsourceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Crowdsource");
                });

            modelBuilder.Entity("GoTravel.API.Domain.Models.Database.GTScoreboardUser", b =>
                {
                    b.HasOne("GoTravel.API.Domain.Models.Database.GTScoreboard", "Scoreboard")
                        .WithMany("Users")
                        .HasForeignKey("ScoreboardUUID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GoTravel.API.Domain.Models.Database.GTUserDetails", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Scoreboard");

                    b.Navigation("User");
                });

            modelBuilder.Entity("GoTravel.API.Domain.Models.Database.GTStopPoint", b =>
                {
                    b.HasOne("GoTravel.API.Domain.Models.Database.GTStopPoint", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("StopPointParentId");

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("GoTravel.API.Domain.Models.Database.GTStopPointInfoValue", b =>
                {
                    b.HasOne("GoTravel.API.Domain.Models.Database.GTStopPointInfoKey", "Key")
                        .WithMany()
                        .HasForeignKey("KeyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GoTravel.API.Domain.Models.Database.GTStopPoint", "StopPoint")
                        .WithMany()
                        .HasForeignKey("StopPointId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Key");

                    b.Navigation("StopPoint");
                });

            modelBuilder.Entity("GoTravel.API.Domain.Models.Database.GTUserFollowings", b =>
                {
                    b.HasOne("GoTravel.API.Domain.Models.Database.GTUserDetails", "Follows")
                        .WithMany("Followers")
                        .HasForeignKey("FollowsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GoTravel.API.Domain.Models.Database.GTUserDetails", "Requester")
                        .WithMany("FollowingUsers")
                        .HasForeignKey("RequesterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Follows");

                    b.Navigation("Requester");
                });

            modelBuilder.Entity("GoTravel.API.Domain.Models.Database.GTUserPointsAudit", b =>
                {
                    b.HasOne("GoTravel.API.Domain.Models.Database.GTUserDetails", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("GoTravel.API.Domain.Models.Database.GLLine", b =>
                {
                    b.Navigation("StopPointLines");
                });

            modelBuilder.Entity("GoTravel.API.Domain.Models.Database.GLLineMode", b =>
                {
                    b.Navigation("Lines");
                });

            modelBuilder.Entity("GoTravel.API.Domain.Models.Database.GTArea", b =>
                {
                    b.Navigation("LineModes");
                });

            modelBuilder.Entity("GoTravel.API.Domain.Models.Database.GTCrowdsourceInfo", b =>
                {
                    b.Navigation("Votes");
                });

            modelBuilder.Entity("GoTravel.API.Domain.Models.Database.GTScoreboard", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("GoTravel.API.Domain.Models.Database.GTStopPoint", b =>
                {
                    b.Navigation("Children");

                    b.Navigation("StopPointLines");
                });

            modelBuilder.Entity("GoTravel.API.Domain.Models.Database.GTUserDetails", b =>
                {
                    b.Navigation("Followers");

                    b.Navigation("FollowingUsers");
                });
#pragma warning restore 612, 618
        }
    }
}
