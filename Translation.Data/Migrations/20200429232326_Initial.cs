using Microsoft.EntityFrameworkCore.Migrations;

namespace Translation.Data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TranslationItems",
                columns: table => new
                {
                    Key = table.Column<string>(type: "varchar(100)", nullable: false),
                    Language = table.Column<string>(type: "varchar(5)", nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TranslationItems", x => x.Key);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TranslationItems");
        }
    }
}
