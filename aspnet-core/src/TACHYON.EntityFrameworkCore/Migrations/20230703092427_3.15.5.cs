using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class _3155 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PackingTypeId",
                table: "ShippingRequestTrips",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSaas",
                table: "RatingLogs",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTrips_PackingTypeId",
                table: "ShippingRequestTrips",
                column: "PackingTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequestTrips_PackingTypes_PackingTypeId",
                table: "ShippingRequestTrips",
                column: "PackingTypeId",
                principalTable: "PackingTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestTrips_PackingTypes_PackingTypeId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequestTrips_PackingTypeId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "PackingTypeId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "IsSaas",
                table: "RatingLogs");
        }
    }
}
