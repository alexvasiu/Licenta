﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MusicIdentifierAPI;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace MusicIdentifierAPI.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("MusicIdentifierAPI.Domain.Playlist", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<bool>("Public");

                    b.Property<string>("ShareLink");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Playlist");
                });

            modelBuilder.Entity("MusicIdentifierAPI.Domain.Song", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("ApparitionDate");

                    b.Property<string>("Artist");

                    b.Property<string>("BeatPortLink");

                    b.Property<double>("Duration");

                    b.Property<string>("Genre");

                    b.Property<int>("IdentificationCounter");

                    b.Property<string>("Name");

                    b.Property<byte[]>("Picture")
                        .HasColumnType("bytea");

                    b.Property<string>("SpotifyLink");

                    b.Property<string>("YoutubeLink");

                    b.HasKey("Id");

                    b.ToTable("Song");
                });

            modelBuilder.Entity("MusicIdentifierAPI.Domain.SongPart", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<double>("Duration");

                    b.Property<string>("Hashtag");

                    b.Property<List<double>>("HighScores");

                    b.Property<List<int>>("Points");

                    b.Property<int>("SongId");

                    b.Property<double>("Time");

                    b.HasKey("Id");

                    b.HasIndex("Points");

                    b.HasIndex("SongId");

                    b.HasIndex("Points", "Time");

                    b.ToTable("SongPart");
                });

            modelBuilder.Entity("MusicIdentifierAPI.Domain.SongPlaylist", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("PlaylistId");

                    b.Property<int>("SongId");

                    b.HasKey("Id");

                    b.HasIndex("PlaylistId");

                    b.HasIndex("SongId");

                    b.ToTable("SongPlaylist");
                });

            modelBuilder.Entity("MusicIdentifierAPI.Domain.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Email");

                    b.Property<string>("FacebookId");

                    b.Property<string>("GoogleId");

                    b.Property<string>("Password");

                    b.Property<string>("RefreshToken");

                    b.Property<int>("UserType");

                    b.Property<string>("Username");

                    b.HasKey("Id");

                    b.ToTable("User");
                });

            modelBuilder.Entity("MusicIdentifierAPI.Domain.Playlist", b =>
                {
                    b.HasOne("MusicIdentifierAPI.Domain.User", "User")
                        .WithMany("Playlists")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MusicIdentifierAPI.Domain.SongPart", b =>
                {
                    b.HasOne("MusicIdentifierAPI.Domain.Song", "Song")
                        .WithMany("SongParts")
                        .HasForeignKey("SongId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MusicIdentifierAPI.Domain.SongPlaylist", b =>
                {
                    b.HasOne("MusicIdentifierAPI.Domain.Playlist", "Playlist")
                        .WithMany("Songs")
                        .HasForeignKey("PlaylistId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MusicIdentifierAPI.Domain.Song", "Song")
                        .WithMany("Playlists")
                        .HasForeignKey("SongId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
