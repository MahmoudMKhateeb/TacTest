using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class addAccidentIdForTripCommentsTbl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestTripAccidentComments_ShippingRequestTripAccidents_AccidentId",
                table: "ShippingRequestTripAccidentComments");

            migrationBuilder.AlterColumn<int>(
                name: "AccidentId",
                table: "ShippingRequestTripAccidentComments",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequestTripAccidentComments_ShippingRequestTripAccidents_AccidentId",
                table: "ShippingRequestTripAccidentComments",
                column: "AccidentId",
                principalTable: "ShippingRequestTripAccidents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestTripAccidentComments_ShippingRequestTripAccidents_AccidentId",
                table: "ShippingRequestTripAccidentComments");

            migrationBuilder.AlterColumn<int>(
                name: "AccidentId",
                table: "ShippingRequestTripAccidentComments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequestTripAccidentComments_ShippingRequestTripAccidents_AccidentId",
                table: "ShippingRequestTripAccidentComments",
                column: "AccidentId",
                principalTable: "ShippingRequestTripAccidents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}