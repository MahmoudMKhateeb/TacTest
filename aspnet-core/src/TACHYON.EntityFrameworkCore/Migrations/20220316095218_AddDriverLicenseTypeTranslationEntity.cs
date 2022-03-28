using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class AddDriverLicenseTypeTranslationEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Key",
                table: "DriverLicenseTypes",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DriverLicenseTypeTranslations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    CoreId = table.Column<int>(nullable: false),
                    Language = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DriverLicenseTypeTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DriverLicenseTypeTranslations_DriverLicenseTypes_CoreId",
                        column: x => x.CoreId,
                        principalTable: "DriverLicenseTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DriverLicenseTypeTranslations_CoreId",
                table: "DriverLicenseTypeTranslations",
                column: "CoreId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DriverLicenseTypeTranslations");

            migrationBuilder.DropColumn(
                name: "Key",
                table: "DriverLicenseTypes");
        }
    }
}