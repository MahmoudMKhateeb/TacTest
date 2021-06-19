using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class changedocumenttypetonullableindocumentfile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentFiles_DocumentTypes_DocumentTypeId",
                table: "DocumentFiles");

            migrationBuilder.AlterColumn<long>(
                name: "DocumentTypeId",
                table: "DocumentFiles",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentFiles_DocumentTypes_DocumentTypeId",
                table: "DocumentFiles",
                column: "DocumentTypeId",
                principalTable: "DocumentTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentFiles_DocumentTypes_DocumentTypeId",
                table: "DocumentFiles");

            migrationBuilder.AlterColumn<long>(
                name: "DocumentTypeId",
                table: "DocumentFiles",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentFiles_DocumentTypes_DocumentTypeId",
                table: "DocumentFiles",
                column: "DocumentTypeId",
                principalTable: "DocumentTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
