using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class Terminologiemonitor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TerminologieEditions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EditionId = table.Column<int>(nullable: true),
                    TerminologieId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TerminologieEditions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TerminologieEditions_AbpEditions_EditionId",
                        column: x => x.EditionId,
                        principalTable: "AbpEditions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TerminologieEditions_AppLocalizations_TerminologieId",
                        column: x => x.TerminologieId,
                        principalTable: "AppLocalizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TerminologiePages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PageUrl = table.Column<string>(nullable: true),
                    TerminologieId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TerminologiePages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TerminologiePages_AppLocalizations_TerminologieId",
                        column: x => x.TerminologieId,
                        principalTable: "AppLocalizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TerminologieEditions_EditionId",
                table: "TerminologieEditions",
                column: "EditionId");

            migrationBuilder.CreateIndex(
                name: "IX_TerminologieEditions_TerminologieId",
                table: "TerminologieEditions",
                column: "TerminologieId");

            migrationBuilder.CreateIndex(
                name: "IX_TerminologiePages_TerminologieId",
                table: "TerminologiePages",
                column: "TerminologieId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TerminologieEditions");

            migrationBuilder.DropTable(
                name: "TerminologiePages");
        }
    }
}
