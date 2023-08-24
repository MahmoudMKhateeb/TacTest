using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class Set_MoiNumber_Nullable_AbpTenant_Table : Migration
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

            migrationBuilder.AddColumn<bool>(
                name: "IsTaxVatIncluded",
                table: "InvoiceNotes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "StampFileType",
                table: "AbpTenants",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StampId",
                table: "AbpTenants",
                nullable: true);

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

            migrationBuilder.DropColumn(
                name: "IsTaxVatIncluded",
                table: "InvoiceNotes");

            migrationBuilder.DropColumn(
                name: "StampFileType",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "StampId",
                table: "AbpTenants");
        }
    }
}
