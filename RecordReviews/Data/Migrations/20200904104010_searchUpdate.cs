using Microsoft.EntityFrameworkCore.Migrations;

namespace RecordReviews.Data.Migrations
{
    public partial class searchUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GenreCountry",
                table: "Search");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Search",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Genre",
                table: "Search",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Country",
                table: "Search");

            migrationBuilder.DropColumn(
                name: "Genre",
                table: "Search");

            migrationBuilder.AddColumn<string>(
                name: "GenreCountry",
                table: "Search",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
