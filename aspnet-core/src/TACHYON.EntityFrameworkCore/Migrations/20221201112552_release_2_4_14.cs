using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class release_2_4_14 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DedicatedShippingRequestDrivers_AbpUsers_DriverUserId",
                table: "DedicatedShippingRequestDrivers");

            migrationBuilder.DropForeignKey(
                name: "FK_PricePackageAppendixes_PricePackageProposals_ProposalId",
                table: "PricePackageAppendixes");

            migrationBuilder.DropForeignKey(
                name: "FK_TmsPricePackages_AbpTenants_ShipperId",
                table: "TmsPricePackages");

            migrationBuilder.DropIndex(
                name: "IX_TmsPricePackages_ShipperId",
                table: "TmsPricePackages");

            migrationBuilder.DropColumn(
                name: "DirectRequestCommission",
                table: "TmsPricePackages");

            migrationBuilder.DropColumn(
                name: "DirectRequestPrice",
                table: "TmsPricePackages");

            migrationBuilder.DropColumn(
                name: "DirectRequestTotalPrice",
                table: "TmsPricePackages");

            migrationBuilder.DropColumn(
                name: "ShipperId",
                table: "TmsPricePackages");

            migrationBuilder.DropColumn(
                name: "TachyonManageCommission",
                table: "TmsPricePackages");

            migrationBuilder.DropColumn(
                name: "TachyonManagePrice",
                table: "TmsPricePackages");

            migrationBuilder.DropColumn(
                name: "TachyonManageTotalPrice",
                table: "TmsPricePackages");

            migrationBuilder.DropColumn(
                name: "ContractNumber",
                table: "PricePackageAppendixes");

            migrationBuilder.AddColumn<int>(
                name: "AppendixId",
                table: "TmsPricePackages",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Commission",
                table: "TmsPricePackages",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "DestinationTenantId",
                table: "TmsPricePackages",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "OfferId",
                table: "TmsPricePackages",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "TmsPricePackages",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "TmsPricePackages",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPrice",
                table: "TmsPricePackages",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "AppendixId",
                table: "PricePackageProposals",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ProposalId",
                table: "PricePackageAppendixes",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "AppendixDate",
                table: "PricePackageAppendixes",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ContractDate",
                table: "PricePackageAppendixes",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "DestinationTenantId",
                table: "PricePackageAppendixes",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsRequestedToReplace",
                table: "DedicatedShippingRequestTrucks",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "OriginalDedicatedTruckId",
                table: "DedicatedShippingRequestTrucks",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReplacementDate",
                table: "DedicatedShippingRequestTrucks",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "ReplacementFlag",
                table: "DedicatedShippingRequestTrucks",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<int>(
                name: "ReplacementIntervalInDays",
                table: "DedicatedShippingRequestTrucks",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReplacementReason",
                table: "DedicatedShippingRequestTrucks",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsRequestedToReplace",
                table: "DedicatedShippingRequestDrivers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "OriginalDedicatedDriverId",
                table: "DedicatedShippingRequestDrivers",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReplacementDate",
                table: "DedicatedShippingRequestDrivers",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "ReplacementFlag",
                table: "DedicatedShippingRequestDrivers",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<int>(
                name: "ReplacementIntervalInDays",
                table: "DedicatedShippingRequestDrivers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReplacementReason",
                table: "DedicatedShippingRequestDrivers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TmsPricePackages_AppendixId",
                table: "TmsPricePackages",
                column: "AppendixId");

            migrationBuilder.CreateIndex(
                name: "IX_TmsPricePackages_DestinationTenantId",
                table: "TmsPricePackages",
                column: "DestinationTenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TmsPricePackages_OfferId",
                table: "TmsPricePackages",
                column: "OfferId");

            migrationBuilder.CreateIndex(
                name: "IX_PricePackageProposals_AppendixId",
                table: "PricePackageProposals",
                column: "AppendixId");

            migrationBuilder.CreateIndex(
                name: "IX_PricePackageAppendixes_DestinationTenantId",
                table: "PricePackageAppendixes",
                column: "DestinationTenantId");

            migrationBuilder.CreateIndex(
                name: "IX_DedicatedShippingRequestTrucks_OriginalDedicatedTruckId",
                table: "DedicatedShippingRequestTrucks",
                column: "OriginalDedicatedTruckId");

            migrationBuilder.CreateIndex(
                name: "IX_DedicatedShippingRequestDrivers_OriginalDedicatedDriverId",
                table: "DedicatedShippingRequestDrivers",
                column: "OriginalDedicatedDriverId");

            migrationBuilder.AddForeignKey(
                name: "FK_DedicatedShippingRequestDrivers_AbpUsers_DriverUserId",
                table: "DedicatedShippingRequestDrivers",
                column: "DriverUserId",
                principalTable: "AbpUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DedicatedShippingRequestDrivers_DedicatedShippingRequestDrivers_OriginalDedicatedDriverId",
                table: "DedicatedShippingRequestDrivers",
                column: "OriginalDedicatedDriverId",
                principalTable: "DedicatedShippingRequestDrivers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DedicatedShippingRequestTrucks_DedicatedShippingRequestTrucks_OriginalDedicatedTruckId",
                table: "DedicatedShippingRequestTrucks",
                column: "OriginalDedicatedTruckId",
                principalTable: "DedicatedShippingRequestTrucks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PricePackageAppendixes_AbpTenants_DestinationTenantId",
                table: "PricePackageAppendixes",
                column: "DestinationTenantId",
                principalTable: "AbpTenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PricePackageAppendixes_PricePackageProposals_ProposalId",
                table: "PricePackageAppendixes",
                column: "ProposalId",
                principalTable: "PricePackageProposals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PricePackageProposals_PricePackageAppendixes_AppendixId",
                table: "PricePackageProposals",
                column: "AppendixId",
                principalTable: "PricePackageAppendixes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TmsPricePackages_PricePackageAppendixes_AppendixId",
                table: "TmsPricePackages",
                column: "AppendixId",
                principalTable: "PricePackageAppendixes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TmsPricePackages_AbpTenants_DestinationTenantId",
                table: "TmsPricePackages",
                column: "DestinationTenantId",
                principalTable: "AbpTenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TmsPricePackages_PriceOffers_OfferId",
                table: "TmsPricePackages",
                column: "OfferId",
                principalTable: "PriceOffers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DedicatedShippingRequestDrivers_AbpUsers_DriverUserId",
                table: "DedicatedShippingRequestDrivers");

            migrationBuilder.DropForeignKey(
                name: "FK_DedicatedShippingRequestDrivers_DedicatedShippingRequestDrivers_OriginalDedicatedDriverId",
                table: "DedicatedShippingRequestDrivers");

            migrationBuilder.DropForeignKey(
                name: "FK_DedicatedShippingRequestTrucks_DedicatedShippingRequestTrucks_OriginalDedicatedTruckId",
                table: "DedicatedShippingRequestTrucks");

            migrationBuilder.DropForeignKey(
                name: "FK_PricePackageAppendixes_AbpTenants_DestinationTenantId",
                table: "PricePackageAppendixes");

            migrationBuilder.DropForeignKey(
                name: "FK_PricePackageAppendixes_PricePackageProposals_ProposalId",
                table: "PricePackageAppendixes");

            migrationBuilder.DropForeignKey(
                name: "FK_PricePackageProposals_PricePackageAppendixes_AppendixId",
                table: "PricePackageProposals");

            migrationBuilder.DropForeignKey(
                name: "FK_TmsPricePackages_PricePackageAppendixes_AppendixId",
                table: "TmsPricePackages");

            migrationBuilder.DropForeignKey(
                name: "FK_TmsPricePackages_AbpTenants_DestinationTenantId",
                table: "TmsPricePackages");

            migrationBuilder.DropForeignKey(
                name: "FK_TmsPricePackages_PriceOffers_OfferId",
                table: "TmsPricePackages");

            migrationBuilder.DropIndex(
                name: "IX_TmsPricePackages_AppendixId",
                table: "TmsPricePackages");

            migrationBuilder.DropIndex(
                name: "IX_TmsPricePackages_DestinationTenantId",
                table: "TmsPricePackages");

            migrationBuilder.DropIndex(
                name: "IX_TmsPricePackages_OfferId",
                table: "TmsPricePackages");

            migrationBuilder.DropIndex(
                name: "IX_PricePackageProposals_AppendixId",
                table: "PricePackageProposals");

            migrationBuilder.DropIndex(
                name: "IX_PricePackageAppendixes_DestinationTenantId",
                table: "PricePackageAppendixes");

            migrationBuilder.DropIndex(
                name: "IX_DedicatedShippingRequestTrucks_OriginalDedicatedTruckId",
                table: "DedicatedShippingRequestTrucks");

            migrationBuilder.DropIndex(
                name: "IX_DedicatedShippingRequestDrivers_OriginalDedicatedDriverId",
                table: "DedicatedShippingRequestDrivers");

            migrationBuilder.DropColumn(
                name: "AppendixId",
                table: "TmsPricePackages");

            migrationBuilder.DropColumn(
                name: "Commission",
                table: "TmsPricePackages");

            migrationBuilder.DropColumn(
                name: "DestinationTenantId",
                table: "TmsPricePackages");

            migrationBuilder.DropColumn(
                name: "OfferId",
                table: "TmsPricePackages");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "TmsPricePackages");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "TmsPricePackages");

            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "TmsPricePackages");

            migrationBuilder.DropColumn(
                name: "AppendixId",
                table: "PricePackageProposals");

            migrationBuilder.DropColumn(
                name: "ContractDate",
                table: "PricePackageAppendixes");

            migrationBuilder.DropColumn(
                name: "DestinationTenantId",
                table: "PricePackageAppendixes");

            migrationBuilder.DropColumn(
                name: "IsRequestedToReplace",
                table: "DedicatedShippingRequestTrucks");

            migrationBuilder.DropColumn(
                name: "OriginalDedicatedTruckId",
                table: "DedicatedShippingRequestTrucks");

            migrationBuilder.DropColumn(
                name: "ReplacementDate",
                table: "DedicatedShippingRequestTrucks");

            migrationBuilder.DropColumn(
                name: "ReplacementFlag",
                table: "DedicatedShippingRequestTrucks");

            migrationBuilder.DropColumn(
                name: "ReplacementIntervalInDays",
                table: "DedicatedShippingRequestTrucks");

            migrationBuilder.DropColumn(
                name: "ReplacementReason",
                table: "DedicatedShippingRequestTrucks");

            migrationBuilder.DropColumn(
                name: "IsRequestedToReplace",
                table: "DedicatedShippingRequestDrivers");

            migrationBuilder.DropColumn(
                name: "OriginalDedicatedDriverId",
                table: "DedicatedShippingRequestDrivers");

            migrationBuilder.DropColumn(
                name: "ReplacementDate",
                table: "DedicatedShippingRequestDrivers");

            migrationBuilder.DropColumn(
                name: "ReplacementFlag",
                table: "DedicatedShippingRequestDrivers");

            migrationBuilder.DropColumn(
                name: "ReplacementIntervalInDays",
                table: "DedicatedShippingRequestDrivers");

            migrationBuilder.DropColumn(
                name: "ReplacementReason",
                table: "DedicatedShippingRequestDrivers");

            migrationBuilder.AddColumn<decimal>(
                name: "DirectRequestCommission",
                table: "TmsPricePackages",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DirectRequestPrice",
                table: "TmsPricePackages",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DirectRequestTotalPrice",
                table: "TmsPricePackages",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "ShipperId",
                table: "TmsPricePackages",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TachyonManageCommission",
                table: "TmsPricePackages",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TachyonManagePrice",
                table: "TmsPricePackages",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TachyonManageTotalPrice",
                table: "TmsPricePackages",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<int>(
                name: "ProposalId",
                table: "PricePackageAppendixes",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "AppendixDate",
                table: "PricePackageAppendixes",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AddColumn<int>(
                name: "ContractNumber",
                table: "PricePackageAppendixes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TmsPricePackages_ShipperId",
                table: "TmsPricePackages",
                column: "ShipperId");

            migrationBuilder.AddForeignKey(
                name: "FK_DedicatedShippingRequestDrivers_AbpUsers_DriverUserId",
                table: "DedicatedShippingRequestDrivers",
                column: "DriverUserId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PricePackageAppendixes_PricePackageProposals_ProposalId",
                table: "PricePackageAppendixes",
                column: "ProposalId",
                principalTable: "PricePackageProposals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TmsPricePackages_AbpTenants_ShipperId",
                table: "TmsPricePackages",
                column: "ShipperId",
                principalTable: "AbpTenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
