﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyGolfStatsApi.Db.AppDbContext;

#nullable disable

namespace MyGolfStatsApi.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250530142703_added email to user")]
    partial class addedemailtouser
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("MyGolfStatsApi.Db.Models.Bag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("AWedge")
                        .HasColumnType("bit");

                    b.Property<bool>("Driver")
                        .HasColumnType("bit");

                    b.Property<bool>("EightI")
                        .HasColumnType("bit");

                    b.Property<bool>("FiveHy")
                        .HasColumnType("bit");

                    b.Property<bool>("FiveI")
                        .HasColumnType("bit");

                    b.Property<bool>("FiveW")
                        .HasColumnType("bit");

                    b.Property<bool>("FourHy")
                        .HasColumnType("bit");

                    b.Property<bool>("FourI")
                        .HasColumnType("bit");

                    b.Property<bool>("FourW")
                        .HasColumnType("bit");

                    b.Property<bool>("GWedge")
                        .HasColumnType("bit");

                    b.Property<bool>("LWedge")
                        .HasColumnType("bit");

                    b.Property<bool>("NineI")
                        .HasColumnType("bit");

                    b.Property<bool>("OneI")
                        .HasColumnType("bit");

                    b.Property<bool>("PWedge")
                        .HasColumnType("bit");

                    b.Property<bool>("SWedge")
                        .HasColumnType("bit");

                    b.Property<bool>("SevenI")
                        .HasColumnType("bit");

                    b.Property<bool>("SevenW")
                        .HasColumnType("bit");

                    b.Property<bool>("SixI")
                        .HasColumnType("bit");

                    b.Property<bool>("SixW")
                        .HasColumnType("bit");

                    b.Property<bool>("ThreeHy")
                        .HasColumnType("bit");

                    b.Property<bool>("ThreeI")
                        .HasColumnType("bit");

                    b.Property<bool>("ThreeW")
                        .HasColumnType("bit");

                    b.Property<bool>("TwoHy")
                        .HasColumnType("bit");

                    b.Property<bool>("TwoI")
                        .HasColumnType("bit");

                    b.Property<bool>("TwoW")
                        .HasColumnType("bit");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("Bag");
                });

            modelBuilder.Entity("MyGolfStatsApi.Db.Models.Course", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClubName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<decimal?>("CourseRating")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("Par")
                        .HasColumnType("int");

                    b.PrimitiveCollection<string>("Tees")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Courses");
                });

            modelBuilder.Entity("MyGolfStatsApi.Db.Models.Hole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CourseId")
                        .HasColumnType("int");

                    b.Property<int>("HoleNumber")
                        .HasColumnType("int");

                    b.Property<int>("Par")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CourseId");

                    b.ToTable("Holes");
                });

            modelBuilder.Entity("MyGolfStatsApi.Db.Models.PenaltyCause", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Cause")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("Club")
                        .IsRequired()
                        .HasMaxLength(15)
                        .HasColumnType("varchar(15)");

                    b.Property<int>("PenaltyStrokes")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("PenaltyCauses");
                });

            modelBuilder.Entity("MyGolfStatsApi.Db.Models.Round", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("BruttoScore")
                        .HasColumnType("int");

                    b.Property<DateOnly>("Date")
                        .HasColumnType("date");

                    b.Property<int>("HolesPlayed")
                        .HasColumnType("int");

                    b.Property<int>("NettoScore")
                        .HasColumnType("int");

                    b.Property<string>("Tee")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Rounds");
                });

            modelBuilder.Entity("MyGolfStatsApi.Db.Models.Settings", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("StatBirdie")
                        .HasColumnType("bit");

                    b.Property<bool>("StatEagle")
                        .HasColumnType("bit");

                    b.Property<bool>("StatFiR")
                        .HasColumnType("bit");

                    b.Property<bool>("StatGiR")
                        .HasColumnType("bit");

                    b.Property<bool>("StatLostBalls")
                        .HasColumnType("bit");

                    b.Property<bool>("StatPenaltyCause")
                        .HasColumnType("bit");

                    b.Property<bool>("StatPenaltyStrokes")
                        .HasColumnType("bit");

                    b.Property<bool>("StatPutts")
                        .HasColumnType("bit");

                    b.Property<bool>("StatSandSave")
                        .HasColumnType("bit");

                    b.Property<bool>("StatUpAndDown")
                        .HasColumnType("bit");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("Settings");
                });

            modelBuilder.Entity("MyGolfStatsApi.Db.Models.Statistics", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool?>("Birdie")
                        .HasColumnType("bit");

                    b.Property<bool?>("Eagle")
                        .HasColumnType("bit");

                    b.Property<string>("FiR")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("GiR")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("HoleId")
                        .HasColumnType("int");

                    b.Property<int?>("LostBalls")
                        .HasColumnType("int");

                    b.Property<int?>("PenaltyStrokes")
                        .HasColumnType("int");

                    b.Property<int?>("Putts")
                        .HasColumnType("int");

                    b.Property<int>("RoundId")
                        .HasColumnType("int");

                    b.Property<bool?>("SandSave")
                        .HasColumnType("bit");

                    b.Property<int>("Strokes")
                        .HasColumnType("int");

                    b.Property<bool?>("UpAndDown")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("HoleId");

                    b.HasIndex("RoundId");

                    b.ToTable("Statistics");
                });

            modelBuilder.Entity("MyGolfStatsApi.Db.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("GolfId")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("varchar(10)");

                    b.Property<decimal>("Hcp")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("varchar(10)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("PenaltyCauseStatistics", b =>
                {
                    b.Property<int>("PenaltyCauseId")
                        .HasColumnType("int");

                    b.Property<int>("StatisticsId")
                        .HasColumnType("int");

                    b.HasKey("PenaltyCauseId", "StatisticsId");

                    b.HasIndex("StatisticsId");

                    b.ToTable("PenaltyCauseStatistics");
                });

            modelBuilder.Entity("MyGolfStatsApi.Db.Models.Bag", b =>
                {
                    b.HasOne("MyGolfStatsApi.Db.Models.User", "User")
                        .WithOne("Bag")
                        .HasForeignKey("MyGolfStatsApi.Db.Models.Bag", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("MyGolfStatsApi.Db.Models.Hole", b =>
                {
                    b.HasOne("MyGolfStatsApi.Db.Models.Course", "Course")
                        .WithMany("Holes")
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Course");
                });

            modelBuilder.Entity("MyGolfStatsApi.Db.Models.Round", b =>
                {
                    b.HasOne("MyGolfStatsApi.Db.Models.User", "User")
                        .WithMany("Rounds")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("MyGolfStatsApi.Db.Models.Settings", b =>
                {
                    b.HasOne("MyGolfStatsApi.Db.Models.User", "User")
                        .WithOne("Settings")
                        .HasForeignKey("MyGolfStatsApi.Db.Models.Settings", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("MyGolfStatsApi.Db.Models.Statistics", b =>
                {
                    b.HasOne("MyGolfStatsApi.Db.Models.Hole", "Hole")
                        .WithMany()
                        .HasForeignKey("HoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MyGolfStatsApi.Db.Models.Round", "Round")
                        .WithMany("Statistics")
                        .HasForeignKey("RoundId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Hole");

                    b.Navigation("Round");
                });

            modelBuilder.Entity("PenaltyCauseStatistics", b =>
                {
                    b.HasOne("MyGolfStatsApi.Db.Models.PenaltyCause", null)
                        .WithMany()
                        .HasForeignKey("PenaltyCauseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MyGolfStatsApi.Db.Models.Statistics", null)
                        .WithMany()
                        .HasForeignKey("StatisticsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MyGolfStatsApi.Db.Models.Course", b =>
                {
                    b.Navigation("Holes");
                });

            modelBuilder.Entity("MyGolfStatsApi.Db.Models.Round", b =>
                {
                    b.Navigation("Statistics");
                });

            modelBuilder.Entity("MyGolfStatsApi.Db.Models.User", b =>
                {
                    b.Navigation("Bag")
                        .IsRequired();

                    b.Navigation("Rounds");

                    b.Navigation("Settings")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
