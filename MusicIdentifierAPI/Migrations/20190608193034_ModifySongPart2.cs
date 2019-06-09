using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MusicIdentifierAPI.Migrations
{
    public partial class ModifySongPart2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<List<int>>(
                name: "Points",
                table: "SongPart",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Points",
                table: "SongPart");
        }
    }
}
