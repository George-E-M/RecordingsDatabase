using Microsoft.EntityFrameworkCore.Migrations;

namespace RecordingsDatabase.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Recording",
                columns: table => new
                {
                    ID = table.Column<string>(nullable: false),
                    Word = table.Column<string>(nullable: true),
                    Syllables = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: true),
                    Tags = table.Column<string>(nullable: true),
                    Uploaded = table.Column<string>(nullable: true),
                    RecordingLength = table.Column<string>(nullable: true),
                    Rating = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recording", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Recording");
        }
    }
}
