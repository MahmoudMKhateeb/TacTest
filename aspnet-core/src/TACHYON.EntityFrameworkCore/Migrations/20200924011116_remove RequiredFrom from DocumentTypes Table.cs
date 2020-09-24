using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class removeRequiredFromfromDocumentTypesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequiredFrom",
                table: "DocumentTypes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RequiredFrom",
                table: "DocumentTypes",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);
        }
    }
}
