using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class RenameInvoiceNoteItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceNoteItem_InvoiceNotes_InvoiceNoteId",
                table: "InvoiceNoteItem");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceNoteItem_ShippingRequestTrips_TripId",
                table: "InvoiceNoteItem");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceNoteItem_ShippingRequestTripVases_TripVasId",
                table: "InvoiceNoteItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InvoiceNoteItem",
                table: "InvoiceNoteItem");

            migrationBuilder.RenameTable(
                name: "InvoiceNoteItem",
                newName: "InvoiceNoteItems");

            migrationBuilder.RenameIndex(
                name: "IX_InvoiceNoteItem_TripVasId",
                table: "InvoiceNoteItems",
                newName: "IX_InvoiceNoteItems_TripVasId");

            migrationBuilder.RenameIndex(
                name: "IX_InvoiceNoteItem_TripId",
                table: "InvoiceNoteItems",
                newName: "IX_InvoiceNoteItems_TripId");

            migrationBuilder.RenameIndex(
                name: "IX_InvoiceNoteItem_InvoiceNoteId",
                table: "InvoiceNoteItems",
                newName: "IX_InvoiceNoteItems_InvoiceNoteId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InvoiceNoteItems",
                table: "InvoiceNoteItems",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceNoteItems_InvoiceNotes_InvoiceNoteId",
                table: "InvoiceNoteItems",
                column: "InvoiceNoteId",
                principalTable: "InvoiceNotes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceNoteItems_ShippingRequestTrips_TripId",
                table: "InvoiceNoteItems",
                column: "TripId",
                principalTable: "ShippingRequestTrips",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceNoteItems_ShippingRequestTripVases_TripVasId",
                table: "InvoiceNoteItems",
                column: "TripVasId",
                principalTable: "ShippingRequestTripVases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceNoteItems_InvoiceNotes_InvoiceNoteId",
                table: "InvoiceNoteItems");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceNoteItems_ShippingRequestTrips_TripId",
                table: "InvoiceNoteItems");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceNoteItems_ShippingRequestTripVases_TripVasId",
                table: "InvoiceNoteItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InvoiceNoteItems",
                table: "InvoiceNoteItems");

            migrationBuilder.RenameTable(
                name: "InvoiceNoteItems",
                newName: "InvoiceNoteItem");

            migrationBuilder.RenameIndex(
                name: "IX_InvoiceNoteItems_TripVasId",
                table: "InvoiceNoteItem",
                newName: "IX_InvoiceNoteItem_TripVasId");

            migrationBuilder.RenameIndex(
                name: "IX_InvoiceNoteItems_TripId",
                table: "InvoiceNoteItem",
                newName: "IX_InvoiceNoteItem_TripId");

            migrationBuilder.RenameIndex(
                name: "IX_InvoiceNoteItems_InvoiceNoteId",
                table: "InvoiceNoteItem",
                newName: "IX_InvoiceNoteItem_InvoiceNoteId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InvoiceNoteItem",
                table: "InvoiceNoteItem",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceNoteItem_InvoiceNotes_InvoiceNoteId",
                table: "InvoiceNoteItem",
                column: "InvoiceNoteId",
                principalTable: "InvoiceNotes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
    }
}
