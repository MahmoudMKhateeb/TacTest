using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class TruckStatusesremoveTenantID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TruckStatuses_TenantId",
                table: "TruckStatuses");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "TruckStatuses");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "TruckStatuses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TruckStatuses_TenantId",
                table: "TruckStatuses",
                column: "TenantId");
        }
    }
}
