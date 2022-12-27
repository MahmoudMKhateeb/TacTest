using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class _2426 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "CommissionType",
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
                name: "IsActive",
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

            migrationBuilder.DropColumn(
                name: "Version",
                table: "PricePackageAppendixes");

            migrationBuilder.AddColumn<int>(
                name: "AppendixId",
                table: "TmsPricePackages",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DestinationTenantId",
                table: "TmsPricePackages",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShippingTypeId",
                table: "TmsPricePackages",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPrice",
                table: "TmsPricePackages",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "SealNumber",
                table: "ShippingRequestTrips",
                nullable: true);

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

            migrationBuilder.AddColumn<int>(
                name: "DestinationTenantId",
                table: "PricePackageAppendixes",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "PricePackageAppendixes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MajorVersion",
                table: "PricePackageAppendixes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MinorVersion",
                table: "PricePackageAppendixes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AppendixId",
                table: "NormalPricePackages",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DriverUserId",
                table: "DedicatedShippingRequestTrucks",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "InvoiceId",
                table: "DedicatedShippingRequestTrucks",
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

            migrationBuilder.AddColumn<long>(
                name: "SubmitInvoiceId",
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

            migrationBuilder.AddColumn<int>(
                name: "AllNumberDays",
                table: "DedicatedDynamicInvoiceItems",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "TmsPriceOfferPackageOffers",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    PriceOfferId = table.Column<long>(nullable: true),
                    DirectRequestId = table.Column<long>(nullable: true),
                    TmsPricePackageId = table.Column<int>(nullable: true),
                    NormalPricePackageId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TmsPriceOfferPackageOffers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TmsPriceOfferPackageOffers_ShippingRequestDirectRequests_DirectRequestId",
                        column: x => x.DirectRequestId,
                        principalTable: "ShippingRequestDirectRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TmsPriceOfferPackageOffers_NormalPricePackages_NormalPricePackageId",
                        column: x => x.NormalPricePackageId,
                        principalTable: "NormalPricePackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TmsPriceOfferPackageOffers_PriceOffers_PriceOfferId",
                        column: x => x.PriceOfferId,
                        principalTable: "PriceOffers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TmsPriceOfferPackageOffers_TmsPricePackages_TmsPricePackageId",
                        column: x => x.TmsPricePackageId,
                        principalTable: "TmsPricePackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TmsPricePackages_AppendixId",
                table: "TmsPricePackages",
                column: "AppendixId");

            migrationBuilder.CreateIndex(
                name: "IX_TmsPricePackages_DestinationTenantId",
                table: "TmsPricePackages",
                column: "DestinationTenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TmsPricePackages_ShippingTypeId",
                table: "TmsPricePackages",
                column: "ShippingTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PricePackageProposals_AppendixId",
                table: "PricePackageProposals",
                column: "AppendixId");

            migrationBuilder.CreateIndex(
                name: "IX_PricePackageAppendixes_DestinationTenantId",
                table: "PricePackageAppendixes",
                column: "DestinationTenantId");

            migrationBuilder.CreateIndex(
                name: "IX_NormalPricePackages_AppendixId",
                table: "NormalPricePackages",
                column: "AppendixId");

            migrationBuilder.CreateIndex(
                name: "IX_DedicatedShippingRequestTrucks_InvoiceId",
                table: "DedicatedShippingRequestTrucks",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_DedicatedShippingRequestTrucks_OriginalDedicatedTruckId",
                table: "DedicatedShippingRequestTrucks",
                column: "OriginalDedicatedTruckId");

            migrationBuilder.CreateIndex(
                name: "IX_DedicatedShippingRequestTrucks_SubmitInvoiceId",
                table: "DedicatedShippingRequestTrucks",
                column: "SubmitInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_DedicatedShippingRequestDrivers_OriginalDedicatedDriverId",
                table: "DedicatedShippingRequestDrivers",
                column: "OriginalDedicatedDriverId");

            migrationBuilder.CreateIndex(
                name: "IX_TmsPriceOfferPackageOffers_DirectRequestId",
                table: "TmsPriceOfferPackageOffers",
                column: "DirectRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_TmsPriceOfferPackageOffers_NormalPricePackageId",
                table: "TmsPriceOfferPackageOffers",
                column: "NormalPricePackageId");

            migrationBuilder.CreateIndex(
                name: "IX_TmsPriceOfferPackageOffers_PriceOfferId",
                table: "TmsPriceOfferPackageOffers",
                column: "PriceOfferId");

            migrationBuilder.CreateIndex(
                name: "IX_TmsPriceOfferPackageOffers_TmsPricePackageId",
                table: "TmsPriceOfferPackageOffers",
                column: "TmsPricePackageId");

            migrationBuilder.AddForeignKey(
                name: "FK_DedicatedShippingRequestDrivers_DedicatedShippingRequestDrivers_OriginalDedicatedDriverId",
                table: "DedicatedShippingRequestDrivers",
                column: "OriginalDedicatedDriverId",
                principalTable: "DedicatedShippingRequestDrivers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DedicatedShippingRequestTrucks_Invoices_InvoiceId",
                table: "DedicatedShippingRequestTrucks",
                column: "InvoiceId",
                principalTable: "Invoices",
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
                name: "FK_DedicatedShippingRequestTrucks_SubmitInvoices_SubmitInvoiceId",
                table: "DedicatedShippingRequestTrucks",
                column: "SubmitInvoiceId",
                principalTable: "SubmitInvoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_NormalPricePackages_PricePackageAppendixes_AppendixId",
                table: "NormalPricePackages",
                column: "AppendixId",
                principalTable: "PricePackageAppendixes",
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
                name: "FK_TmsPricePackages_ShippingTypes_ShippingTypeId",
                table: "TmsPricePackages",
                column: "ShippingTypeId",
                principalTable: "ShippingTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DedicatedShippingRequestDrivers_DedicatedShippingRequestDrivers_OriginalDedicatedDriverId",
                table: "DedicatedShippingRequestDrivers");

            migrationBuilder.DropForeignKey(
                name: "FK_DedicatedShippingRequestTrucks_Invoices_InvoiceId",
                table: "DedicatedShippingRequestTrucks");

            migrationBuilder.DropForeignKey(
                name: "FK_DedicatedShippingRequestTrucks_DedicatedShippingRequestTrucks_OriginalDedicatedTruckId",
                table: "DedicatedShippingRequestTrucks");

            migrationBuilder.DropForeignKey(
                name: "FK_DedicatedShippingRequestTrucks_SubmitInvoices_SubmitInvoiceId",
                table: "DedicatedShippingRequestTrucks");

            migrationBuilder.DropForeignKey(
                name: "FK_NormalPricePackages_PricePackageAppendixes_AppendixId",
                table: "NormalPricePackages");

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
                name: "FK_TmsPricePackages_ShippingTypes_ShippingTypeId",
                table: "TmsPricePackages");

            migrationBuilder.DropTable(
                name: "TmsPriceOfferPackageOffers");

            migrationBuilder.DropIndex(
                name: "IX_TmsPricePackages_AppendixId",
                table: "TmsPricePackages");

            migrationBuilder.DropIndex(
                name: "IX_TmsPricePackages_DestinationTenantId",
                table: "TmsPricePackages");

            migrationBuilder.DropIndex(
                name: "IX_TmsPricePackages_ShippingTypeId",
                table: "TmsPricePackages");

            migrationBuilder.DropIndex(
                name: "IX_PricePackageProposals_AppendixId",
                table: "PricePackageProposals");

            migrationBuilder.DropIndex(
                name: "IX_PricePackageAppendixes_DestinationTenantId",
                table: "PricePackageAppendixes");

            migrationBuilder.DropIndex(
                name: "IX_NormalPricePackages_AppendixId",
                table: "NormalPricePackages");

            migrationBuilder.DropIndex(
                name: "IX_DedicatedShippingRequestTrucks_InvoiceId",
                table: "DedicatedShippingRequestTrucks");

            migrationBuilder.DropIndex(
                name: "IX_DedicatedShippingRequestTrucks_OriginalDedicatedTruckId",
                table: "DedicatedShippingRequestTrucks");

            migrationBuilder.DropIndex(
                name: "IX_DedicatedShippingRequestTrucks_SubmitInvoiceId",
                table: "DedicatedShippingRequestTrucks");

            migrationBuilder.DropIndex(
                name: "IX_DedicatedShippingRequestDrivers_OriginalDedicatedDriverId",
                table: "DedicatedShippingRequestDrivers");

            migrationBuilder.DropColumn(
                name: "AppendixId",
                table: "TmsPricePackages");

            migrationBuilder.DropColumn(
                name: "DestinationTenantId",
                table: "TmsPricePackages");

            migrationBuilder.DropColumn(
                name: "ShippingTypeId",
                table: "TmsPricePackages");

            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "TmsPricePackages");

            migrationBuilder.DropColumn(
                name: "SealNumber",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "AppendixId",
                table: "PricePackageProposals");

            migrationBuilder.DropColumn(
                name: "DestinationTenantId",
                table: "PricePackageAppendixes");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "PricePackageAppendixes");

            migrationBuilder.DropColumn(
                name: "MajorVersion",
                table: "PricePackageAppendixes");

            migrationBuilder.DropColumn(
                name: "MinorVersion",
                table: "PricePackageAppendixes");

            migrationBuilder.DropColumn(
                name: "AppendixId",
                table: "NormalPricePackages");

            migrationBuilder.DropColumn(
                name: "DriverUserId",
                table: "DedicatedShippingRequestTrucks");

            migrationBuilder.DropColumn(
                name: "InvoiceId",
                table: "DedicatedShippingRequestTrucks");

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
                name: "SubmitInvoiceId",
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

            migrationBuilder.DropColumn(
                name: "AllNumberDays",
                table: "DedicatedDynamicInvoiceItems");

            migrationBuilder.AddColumn<int>(
                name: "CommissionType",
                table: "TmsPricePackages",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "TmsPricePackages",
                type: "bit",
                nullable: false,
                defaultValue: false);

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

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "PricePackageAppendixes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TmsPricePackages_ShipperId",
                table: "TmsPricePackages",
                column: "ShipperId");

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
