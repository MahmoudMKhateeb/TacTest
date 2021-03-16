using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class AddPackingTypeToShippingRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PackingType",
                table: "ShippingRequests");

            migrationBuilder.AddColumn<int>(
                name: "PackingTypeId",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequests_PackingTypeId",
                table: "ShippingRequests",
                column: "PackingTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequests_PackingTypes_PackingTypeId",
                table: "ShippingRequests",
                column: "PackingTypeId",
                principalTable: "PackingTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_PackingTypes_PackingTypeId",
                table: "ShippingRequests");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequests_PackingTypeId",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "PackingTypeId",
                table: "ShippingRequests");

            migrationBuilder.AddColumn<string>(
                name: "PackingType",
                table: "ShippingRequests",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
