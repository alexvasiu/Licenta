using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace MusicIdentifierAPI.Migrations
{
    public partial class FinishDb1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "User",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FacebookId",
                table: "User",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GoogleId",
                table: "User",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "User",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Song",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(nullable: true),
                    Artist = table.Column<string>(nullable: true),
                    Genre = table.Column<string>(nullable: true),
                    FileData = table.Column<List<byte>>(nullable: true),
                    ApparitionDate = table.Column<DateTime>(nullable: false),
                    IdentificationCounter = table.Column<int>(nullable: false),
                    YoutubeLink = table.Column<string>(nullable: true),
                    SpotifyLink = table.Column<string>(nullable: true),
                    Duration = table.Column<int>(nullable: false),
                    Picture = table.Column<Byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Song", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Playlist",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    UserId = table.Column<int>(nullable: false),
                    SongId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Public = table.Column<bool>(nullable: false),
                    ShareLink = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Playlist", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Playlist_Song_SongId",
                        column: x => x.SongId,
                        principalTable: "Song",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Playlist_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SongPart",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Hashtag = table.Column<string>(nullable: true),
                    Time = table.Column<double>(nullable: false),
                    SongId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SongPart", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SongPart_Song_SongId",
                        column: x => x.SongId,
                        principalTable: "Song",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Playlist_SongId",
                table: "Playlist",
                column: "SongId");

            migrationBuilder.CreateIndex(
                name: "IX_Playlist_UserId",
                table: "Playlist",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SongPart_SongId",
                table: "SongPart",
                column: "SongId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Playlist");

            migrationBuilder.DropTable(
                name: "SongPart");

            migrationBuilder.DropTable(
                name: "Song");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "User");

            migrationBuilder.DropColumn(
                name: "FacebookId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "GoogleId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "User");
        }
    }
}
