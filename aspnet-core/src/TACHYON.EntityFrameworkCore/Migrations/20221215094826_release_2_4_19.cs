using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class release_2_4_19 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TmsPricePackages_PriceOffers_OfferId",
                table: "TmsPricePackages");

            migrationBuilder.DropIndex(
                name: "IX_TmsPricePackages_OfferId",
                table: "TmsPricePackages");

            migrationBuilder.DropColumn(
                name: "OfferId",
                table: "TmsPricePackages");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "TmsPricePackages");

            migrationBuilder.AddColumn<long>(
                name: "InvoiceId",
                table: "DedicatedShippingRequestTrucks",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SubmitInvoiceId",
                table: "DedicatedShippingRequestTrucks",
                nullable: true);

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
                name: "IX_DedicatedShippingRequestTrucks_InvoiceId",
                table: "DedicatedShippingRequestTrucks",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_DedicatedShippingRequestTrucks_SubmitInvoiceId",
                table: "DedicatedShippingRequestTrucks",
                column: "SubmitInvoiceId");

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
                name: "FK_DedicatedShippingRequestTrucks_Invoices_InvoiceId",
                table: "DedicatedShippingRequestTrucks",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DedicatedShippingRequestTrucks_SubmitInvoices_SubmitInvoiceId",
                table: "DedicatedShippingRequestTrucks",
                column: "SubmitInvoiceId",
                principalTable: "SubmitInvoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DedicatedShippingRequestTrucks_Invoices_InvoiceId",
                table: "DedicatedShippingRequestTrucks");

            migrationBuilder.DropForeignKey(
                name: "FK_DedicatedShippingRequestTrucks_SubmitInvoices_SubmitInvoiceId",
                table: "DedicatedShippingRequestTrucks");

            migrationBuilder.DropTable(
                name: "TmsPriceOfferPackageOffers");

            migrationBuilder.DropIndex(
                name: "IX_DedicatedShippingRequestTrucks_InvoiceId",
                table: "DedicatedShippingRequestTrucks");

            migrationBuilder.DropIndex(
                name: "IX_DedicatedShippingRequestTrucks_SubmitInvoiceId",
                table: "DedicatedShippingRequestTrucks");

            migrationBuilder.DropColumn(
                name: "InvoiceId",
                table: "DedicatedShippingRequestTrucks");

            migrationBuilder.DropColumn(
                name: "SubmitInvoiceId",
                table: "DedicatedShippingRequestTrucks");

            migrationBuilder.AddColumn<long>(
                name: "OfferId",
                table: "TmsPricePackages",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "TmsPricePackages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TmsPricePackages_OfferId",
                table: "TmsPricePackages",
                column: "OfferId");

            migrationBuilder.AddForeignKey(
                name: "FK_TmsPricePackages_PriceOffers_OfferId",
                table: "TmsPricePackages",
                column: "OfferId",
                principalTable: "PriceOffers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
