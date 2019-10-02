using Microsoft.EntityFrameworkCore.Migrations;

namespace WebParserTestApp2.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GooglePlayTable",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    Link = table.Column<string>(nullable: true),
                    IconLink = table.Column<string>(nullable: true),
                    Rating = table.Column<string>(nullable: true),
                    SearchQuery = table.Column<string>(nullable: true),
                    ScreenshotsSum = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GooglePlayTable", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppStoreTable",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    Link = table.Column<string>(nullable: true),
                    IconLink = table.Column<string>(nullable: true),
                    Rating = table.Column<string>(nullable: true),
                    SearchQuery = table.Column<string>(nullable: true),
                    ScreenshotsSum = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppStoreTable", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MainTable",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SearchQuery = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MainTable", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppStoreTable");
            migrationBuilder.DropTable(
                name: "GooglePlayTable");
            migrationBuilder.DropTable(
                name: "MainTable");
        }
    }
}
