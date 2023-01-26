using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class incidenttripflowhotfix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ForceContinueTripEnabled",
                table: "ShippingRequestTripAccidents",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte>(
                name: "LastPointStatus",
                table: "ShippingRequestTripAccidents",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<int>(
                name: "ResolveId",
                table: "ShippingRequestTripAccidents",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ShippingRequestTripAccidentResolves",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsTripImpactEnabled",
                table: "ShippingRequestReasonAccidents",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTripAccidents_ResolveId",
                table: "ShippingRequestTripAccidents",
                column: "ResolveId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequestTripAccidents_ShippingRequestTripAccidentResolves_ResolveId",
                table: "ShippingRequestTripAccidents",
                column: "ResolveId",
                principalTable: "ShippingRequestTripAccidentResolves",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestTripAccidents_ShippingRequestTripAccidentResolves_ResolveId",
                table: "ShippingRequestTripAccidents");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequestTripAccidents_ResolveId",
                table: "ShippingRequestTripAccidents");

            migrationBuilder.DropColumn(
                name: "ForceContinueTripEnabled",
                table: "ShippingRequestTripAccidents");

            migrationBuilder.DropColumn(
                name: "LastPointStatus",
                table: "ShippingRequestTripAccidents");

            migrationBuilder.DropColumn(
                name: "ResolveId",
                table: "ShippingRequestTripAccidents");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "ShippingRequestTripAccidentResolves");

            migrationBuilder.DropColumn(
                name: "IsTripImpactEnabled",
                table: "ShippingRequestReasonAccidents");
        }
    }
}
