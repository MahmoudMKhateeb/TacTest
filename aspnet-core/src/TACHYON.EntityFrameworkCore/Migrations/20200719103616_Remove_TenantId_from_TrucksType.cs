using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class Remove_TenantId_from_TrucksType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TrucksTypes_TenantId",
                table: "TrucksTypes");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "TrucksTypes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "TrucksTypes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrucksTypes_TenantId",
                table: "TrucksTypes",
                column: "TenantId");
        }
    }
}
