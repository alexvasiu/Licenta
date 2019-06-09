using Microsoft.EntityFrameworkCore.Migrations;

namespace MusicIdentifierAPI.Migrations
{
    public partial class DurationToDouble : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Duration",
                table: "Song",
                nullable: false,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Duration",
                table: "Song",
                nullable: false,
                oldClrType: typeof(double));
        }
    }
}
