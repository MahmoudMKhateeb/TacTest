using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class Add_ReplacedDriverId_to_Trip : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ReplacesDriverId",
                table: "ShippingRequestTrips",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTrips_ReplacesDriverId",
                table: "ShippingRequestTrips",
                column: "ReplacesDriverId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequestTrips_AbpUsers_ReplacesDriverId",
                table: "ShippingRequestTrips",
                column: "ReplacesDriverId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestTrips_AbpUsers_ReplacesDriverId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequestTrips_ReplacesDriverId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "ReplacesDriverId",
                table: "ShippingRequestTrips");
        }
    }
}
