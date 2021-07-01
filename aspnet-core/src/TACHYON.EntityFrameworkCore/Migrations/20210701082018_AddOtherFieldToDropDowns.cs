using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class AddOtherFieldToDropDowns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OtherVasName",
                table: "ShippingRequestVases",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherReasonName",
                table: "ShippingRequestTripAccidents",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherGoodsCategoryName",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherPackingTypeName",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherTransportTypeName",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherTrucksTypeName",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherGoodsCategoryName",
                table: "GoodsDetails",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherUnitOfMeasureName",
                table: "GoodsDetails",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherDocumentTypeName",
                table: "DocumentFiles",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OtherVasName",
                table: "ShippingRequestVases");

            migrationBuilder.DropColumn(
                name: "OtherReasonName",
                table: "ShippingRequestTripAccidents");

            migrationBuilder.DropColumn(
                name: "OtherGoodsCategoryName",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "OtherPackingTypeName",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "OtherTransportTypeName",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "OtherTrucksTypeName",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "OtherGoodsCategoryName",
                table: "GoodsDetails");

            migrationBuilder.DropColumn(
                name: "OtherUnitOfMeasureName",
                table: "GoodsDetails");

            migrationBuilder.DropColumn(
                name: "OtherDocumentTypeName",
                table: "DocumentFiles");
        }
    }
}
