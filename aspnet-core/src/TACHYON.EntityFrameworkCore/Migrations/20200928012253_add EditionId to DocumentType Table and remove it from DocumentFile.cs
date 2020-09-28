using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class addEditionIdtoDocumentTypeTableandremoveitfromDocumentFile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentFiles_AbpEditions_EditionId",
                table: "DocumentFiles");

            migrationBuilder.DropIndex(
                name: "IX_DocumentFiles_EditionId",
                table: "DocumentFiles");

            migrationBuilder.DropColumn(
                name: "EditionId",
                table: "DocumentFiles");

            migrationBuilder.AddColumn<int>(
                name: "EditionId",
                table: "DocumentTypes",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTypes_EditionId",
                table: "DocumentTypes",
                column: "EditionId");

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentTypes_AbpEditions_EditionId",
                table: "DocumentTypes",
                column: "EditionId",
                principalTable: "AbpEditions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentTypes_AbpEditions_EditionId",
                table: "DocumentTypes");

            migrationBuilder.DropIndex(
                name: "IX_DocumentTypes_EditionId",
                table: "DocumentTypes");

            migrationBuilder.DropColumn(
                name: "EditionId",
                table: "DocumentTypes");

            migrationBuilder.AddColumn<int>(
                name: "EditionId",
                table: "DocumentFiles",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentFiles_EditionId",
                table: "DocumentFiles",
                column: "EditionId");

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentFiles_AbpEditions_EditionId",
                table: "DocumentFiles",
                column: "EditionId",
                principalTable: "AbpEditions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
