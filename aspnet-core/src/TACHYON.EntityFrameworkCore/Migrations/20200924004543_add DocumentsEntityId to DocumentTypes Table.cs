using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class addDocumentsEntityIdtoDocumentTypesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DocumentsEntityId",
                table: "DocumentTypes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTypes_DocumentsEntityId",
                table: "DocumentTypes",
                column: "DocumentsEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentTypes_DocumentsEntities_DocumentsEntityId",
                table: "DocumentTypes",
                column: "DocumentsEntityId",
                principalTable: "DocumentsEntities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentTypes_DocumentsEntities_DocumentsEntityId",
                table: "DocumentTypes");

            migrationBuilder.DropIndex(
                name: "IX_DocumentTypes_DocumentsEntityId",
                table: "DocumentTypes");

            migrationBuilder.DropColumn(
                name: "DocumentsEntityId",
                table: "DocumentTypes");
        }
    }
}
