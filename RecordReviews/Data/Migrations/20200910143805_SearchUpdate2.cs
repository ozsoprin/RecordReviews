using Microsoft.EntityFrameworkCore.Migrations;

namespace RecordReviews.Data.Migrations
{
    public partial class SearchUpdate2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ArtistName",
                table: "Search",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArtistName",
                table: "Search");
        }
    }
}
