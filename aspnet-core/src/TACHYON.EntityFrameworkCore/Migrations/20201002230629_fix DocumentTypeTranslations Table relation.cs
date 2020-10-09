using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class fixDocumentTypeTranslationsTablerelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentTypeTranslations_DocumentTypes_CoreId1",
                table: "DocumentTypeTranslations");

            migrationBuilder.DropIndex(
                name: "IX_DocumentTypeTranslations_CoreId1",
                table: "DocumentTypeTranslations");

            migrationBuilder.DropColumn(
                name: "CoreId1",
                table: "DocumentTypeTranslations");

            migrationBuilder.AlterColumn<long>(
                name: "CoreId",
                table: "DocumentTypeTranslations",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTypeTranslations_CoreId",
                table: "DocumentTypeTranslations",
                column: "CoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentTypeTranslations_DocumentTypes_CoreId",
                table: "DocumentTypeTranslations",
                column: "CoreId",
                principalTable: "DocumentTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentTypeTranslations_DocumentTypes_CoreId",
                table: "DocumentTypeTranslations");

            migrationBuilder.DropIndex(
                name: "IX_DocumentTypeTranslations_CoreId",
                table: "DocumentTypeTranslations");

            migrationBuilder.AlterColumn<int>(
                name: "CoreId",
                table: "DocumentTypeTranslations",
                type: "int",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AddColumn<long>(
                name: "CoreId1",
                table: "DocumentTypeTranslations",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTypeTranslations_CoreId1",
                table: "DocumentTypeTranslations",
                column: "CoreId1");

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentTypeTranslations_DocumentTypes_CoreId1",
                table: "DocumentTypeTranslations",
                column: "CoreId1",
                principalTable: "DocumentTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
