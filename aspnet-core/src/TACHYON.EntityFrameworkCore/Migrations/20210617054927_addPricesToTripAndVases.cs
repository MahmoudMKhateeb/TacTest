using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class addPricesToTripAndVases : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CommissionAmount",
                table: "ShippingRequestTripVases",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "CommissionType",
                table: "ShippingRequestTripVases",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "ShippingRequestTripVases",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotalAmount",
                table: "ShippingRequestTripVases",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotalAmountWithCommission",
                table: "ShippingRequestTripVases",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxVat",
                table: "ShippingRequestTripVases",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                table: "ShippingRequestTripVases",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmountWithCommission",
                table: "ShippingRequestTripVases",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "VatAmount",
                table: "ShippingRequestTripVases",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "VatAmountWithCommission",
                table: "ShippingRequestTripVases",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CommissionAmount",
                table: "ShippingRequestTrips",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "CommissionType",
                table: "ShippingRequestTrips",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotalAmount",
                table: "ShippingRequestTrips",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotalAmountWithCommission",
                table: "ShippingRequestTrips",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxVat",
                table: "ShippingRequestTrips",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                table: "ShippingRequestTrips",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmountWithCommission",
                table: "ShippingRequestTrips",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "VatAmount",
                table: "ShippingRequestTrips",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "VatAmountWithCommission",
                table: "ShippingRequestTrips",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommissionAmount",
                table: "ShippingRequestTripVases");

            migrationBuilder.DropColumn(
                name: "CommissionType",
                table: "ShippingRequestTripVases");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "ShippingRequestTripVases");

            migrationBuilder.DropColumn(
                name: "SubTotalAmount",
                table: "ShippingRequestTripVases");

            migrationBuilder.DropColumn(
                name: "SubTotalAmountWithCommission",
                table: "ShippingRequestTripVases");

            migrationBuilder.DropColumn(
                name: "TaxVat",
                table: "ShippingRequestTripVases");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "ShippingRequestTripVases");

            migrationBuilder.DropColumn(
                name: "TotalAmountWithCommission",
                table: "ShippingRequestTripVases");

            migrationBuilder.DropColumn(
                name: "VatAmount",
                table: "ShippingRequestTripVases");

            migrationBuilder.DropColumn(
                name: "VatAmountWithCommission",
                table: "ShippingRequestTripVases");

            migrationBuilder.DropColumn(
                name: "CommissionAmount",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "CommissionType",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "SubTotalAmount",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "SubTotalAmountWithCommission",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "TaxVat",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "TotalAmountWithCommission",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "VatAmount",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "VatAmountWithCommission",
                table: "ShippingRequestTrips");
        }
    }
}
