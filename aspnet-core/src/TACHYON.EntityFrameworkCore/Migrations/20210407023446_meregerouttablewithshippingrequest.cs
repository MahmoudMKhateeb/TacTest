using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class meregerouttablewithshippingrequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DestinationCityId",
                table: "ShippingRequests",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OriginCityId",
                table: "ShippingRequests",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.AddColumn<byte>(
                name: "RouteTypeId",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequests_DestinationCityId",
                table: "ShippingRequests",
                column: "DestinationCityId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequests_OriginCityId",
                table: "ShippingRequests",
                
                column: "OriginCityId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequests_Cities_DestinationCityId",
                table: "ShippingRequests",
                column: "DestinationCityId",
                principalTable: "Cities",
                principalColumn: "Id",
                
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequests_Cities_OriginCityId",
                table: "ShippingRequests",
                column: "OriginCityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_Cities_DestinationCityId",
                table: "ShippingRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_Cities_OriginCityId",
                table: "ShippingRequests");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequests_DestinationCityId",
                table: "ShippingRequests");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequests_OriginCityId",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "DestinationCityId",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "OriginCityId",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "RouteTypeId",
                table: "ShippingRequests");
        }
    }
}
