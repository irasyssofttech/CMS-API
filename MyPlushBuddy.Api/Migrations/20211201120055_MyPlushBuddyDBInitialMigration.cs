using Microsoft.EntityFrameworkCore.Migrations;

namespace MyPlushBuddy.Api.Migrations
{
    public partial class MyPlushBuddyDBInitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PageTags",
                columns: table => new
                {
                    PageTagId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PageRoute = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    PageTitle = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: false),
                    Robots = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Ogtitle = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: true),
                    Ogtype = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Ogurl = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: true),
                    OgsiteName = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: true),
                    OgarcticlePublisher = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: true),
                    Ogdescription = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: true),
                    Ogimage = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: true),
                    Oglocal = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    TwitterCard = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: true),
                    TwitterCreator = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: true),
                    TwitterSite = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: true),
                    TwitterImage = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: true),
                    PageKeywords = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: true),
                    MetaNameDescription = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageTags", x => x.PageTagId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PageTags");
        }
    }
}
