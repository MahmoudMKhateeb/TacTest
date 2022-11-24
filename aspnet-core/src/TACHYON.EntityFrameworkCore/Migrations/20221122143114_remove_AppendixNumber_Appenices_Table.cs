using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class remove_AppendixNumber_Appenices_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContractNumber",
                table: "PricePackageAppendixes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContractNumber",
                table: "PricePackageAppendixes",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
