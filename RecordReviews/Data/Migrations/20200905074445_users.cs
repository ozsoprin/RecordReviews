using Microsoft.EntityFrameworkCore.Migrations;

namespace RecordReviews.Data.Migrations
{
    public partial class users : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OwnerID",
                table: "Reviews",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Reviews",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "OwnerID",
                table: "Artists",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Artists",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "OwnerID",
                table: "Albums",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Albums",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OwnerID",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "OwnerID",
                table: "Artists");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Artists");

            migrationBuilder.DropColumn(
                name: "OwnerID",
                table: "Albums");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Albums");
        }
    }
}
