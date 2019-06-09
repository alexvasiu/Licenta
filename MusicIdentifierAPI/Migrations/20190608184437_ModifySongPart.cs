using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MusicIdentifierAPI.Migrations
{
    public partial class ModifySongPart : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Duration",
                table: "SongPart",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<List<double>>(
                name: "HighScores",
                table: "SongPart",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Duration",
                table: "SongPart");

            migrationBuilder.DropColumn(
                name: "HighScores",
                table: "SongPart");
        }
    }
}
