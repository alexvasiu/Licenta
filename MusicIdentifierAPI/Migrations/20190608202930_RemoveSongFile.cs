using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MusicIdentifierAPI.Migrations
{
    public partial class RemoveSongFile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileData",
                table: "Song");

            migrationBuilder.CreateIndex(
                name: "IX_SongPart_Points",
                table: "SongPart",
                column: "Points");

            migrationBuilder.CreateIndex(
                name: "IX_SongPart_Points_Time",
                table: "SongPart",
                columns: new[] { "Points", "Time" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SongPart_Points",
                table: "SongPart");

            migrationBuilder.DropIndex(
                name: "IX_SongPart_Points_Time",
                table: "SongPart");

            migrationBuilder.AddColumn<List<byte>>(
                name: "FileData",
                table: "Song",
                nullable: true);
        }
    }
}
