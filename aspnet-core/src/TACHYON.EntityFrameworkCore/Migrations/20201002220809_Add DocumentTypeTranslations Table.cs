using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class AddDocumentTypeTranslationsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DocumentTypeTranslations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 256, nullable: false),
                    Language = table.Column<string>(nullable: true),
                    CoreId1 = table.Column<long>(nullable: true),
                    CoreId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentTypeTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentTypeTranslations_DocumentTypes_CoreId1",
                        column: x => x.CoreId1,
                        principalTable: "DocumentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTypeTranslations_CoreId1",
                table: "DocumentTypeTranslations",
                column: "CoreId1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentTypeTranslations");
        }
    }
}
