using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class AddPrice_InvoiceNoteItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceNoteItem_ShippingRequestTrips_TripId",
                table: "InvoiceNoteItem");

            migrationBuilder.AlterColumn<int>(
                name: "TripId",
                table: "InvoiceNoteItem",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "InvoiceNoteItem",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                table: "InvoiceNoteItem",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<long>(
                name: "TripVasId",
                table: "InvoiceNoteItem",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "VatAmount",
                table: "InvoiceNoteItem",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceNoteItem_TripVasId",
                table: "InvoiceNoteItem",
                column: "TripVasId");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceNoteItem_ShippingRequestTrips_TripId",
                table: "InvoiceNoteItem",
                column: "TripId",
                principalTable: "ShippingRequestTrips",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceNoteItem_ShippingRequestTripVases_TripVasId",
                table: "InvoiceNoteItem",
                column: "TripVasId",
                principalTable: "ShippingRequestTripVases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceNoteItem_ShippingRequestTrips_TripId",
                table: "InvoiceNoteItem");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceNoteItem_ShippingRequestTripVases_TripVasId",
                table: "InvoiceNoteItem");

            migrationBuilder.DropIndex(
                name: "IX_InvoiceNoteItem_TripVasId",
                table: "InvoiceNoteItem");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "InvoiceNoteItem");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "InvoiceNoteItem");

            migrationBuilder.DropColumn(
                name: "TripVasId",
                table: "InvoiceNoteItem");

            migrationBuilder.DropColumn(
                name: "VatAmount",
                table: "InvoiceNoteItem");

            migrationBuilder.AlterColumn<int>(
                name: "TripId",
                table: "InvoiceNoteItem",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceNoteItem_ShippingRequestTrips_TripId",
                table: "InvoiceNoteItem",
                column: "TripId",
                principalTable: "ShippingRequestTrips",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
