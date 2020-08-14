using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RecordReviews.Data.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Albums",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(nullable: false),
                    Artist = table.Column<string>(nullable: false),
                    ArtistId = table.Column<int>(nullable: false),
                    ReleaseDate = table.Column<DateTime>(nullable: false),
                    Genre = table.Column<string>(nullable: false),
                    AvgRate = table.Column<double>(nullable: false),
                    PageViews = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Albums", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Artists",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false),
                    BirthPlace = table.Column<string>(nullable: false),
                    AvgRate = table.Column<double>(nullable: false),
                    PageViews = table.Column<int>(nullable: false),
                    Genre = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Artists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(nullable: false),
                    Username = table.Column<string>(nullable: true),
                    AlbumId = table.Column<int>(nullable: false),
                    AlbumTitle = table.Column<string>(nullable: false),
                    Comment = table.Column<string>(nullable: false),
                    Rate = table.Column<int>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Search",
                columns: table => new
                {
                    type = table.Column<string>(nullable: false),
                    PrimaryKey = table.Column<string>(nullable: true),
                    SecondaryKey = table.Column<string>(nullable: true),
                    MinRate = table.Column<double>(nullable: true),
                    MaxRate = table.Column<double>(nullable: true),
                    GenreCountry = table.Column<string>(nullable: true),
                    MaxDateTime = table.Column<DateTime>(nullable: true),
                    MinDateTime = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Search", x => x.type);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Albums");

            migrationBuilder.DropTable(
                name: "Artists");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "Search");
        }
    }
}
