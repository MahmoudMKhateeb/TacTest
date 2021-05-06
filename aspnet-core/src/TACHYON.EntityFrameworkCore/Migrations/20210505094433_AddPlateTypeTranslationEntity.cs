using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class AddPlateTypeTranslationEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlateTypeTranslations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CoreId = table.Column<int>(nullable: false),
                    Language = table.Column<string>(nullable: true),
                    DisplayName = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlateTypeTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlateTypeTranslations_PlateTypes_CoreId",
                        column: x => x.CoreId,
                        principalTable: "PlateTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlateTypeTranslations_CoreId",
                table: "PlateTypeTranslations",
                column: "CoreId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlateTypeTranslations");
        }
    }
}
