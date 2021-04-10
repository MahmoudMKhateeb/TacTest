using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class addReasoneId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestTripAccidents_ShippingRequestReasonAccidents_ResoneId",
                table: "ShippingRequestTripAccidents");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequestTripAccidents_ResoneId",
                table: "ShippingRequestTripAccidents");

            migrationBuilder.DropColumn(
                name: "ResoneId",
                table: "ShippingRequestTripAccidents");

            migrationBuilder.AddColumn<int>(
                name: "ReasoneId",
                table: "ShippingRequestTripAccidents",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTripAccidents_ReasoneId",
                table: "ShippingRequestTripAccidents",
                column: "ReasoneId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequestTripAccidents_ShippingRequestReasonAccidents_ReasoneId",
                table: "ShippingRequestTripAccidents",
                column: "ReasoneId",
                principalTable: "ShippingRequestReasonAccidents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestTripAccidents_ShippingRequestReasonAccidents_ReasoneId",
                table: "ShippingRequestTripAccidents");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequestTripAccidents_ReasoneId",
                table: "ShippingRequestTripAccidents");

            migrationBuilder.DropColumn(
                name: "ReasoneId",
                table: "ShippingRequestTripAccidents");

            migrationBuilder.AddColumn<int>(
                name: "ResoneId",
                table: "ShippingRequestTripAccidents",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTripAccidents_ResoneId",
                table: "ShippingRequestTripAccidents",
                column: "ResoneId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequestTripAccidents_ShippingRequestReasonAccidents_ResoneId",
                table: "ShippingRequestTripAccidents",
                column: "ResoneId",
                principalTable: "ShippingRequestReasonAccidents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
