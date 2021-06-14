using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class CreatePriceOffers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "PriceOfferId",
                table: "ShippingRequestVasesPricing",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PriceOffers",
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
                    ReferenceNumber = table.Column<long>(nullable: true),
                    ParentId = table.Column<long>(nullable: true),
                    ShippingRequestId = table.Column<long>(nullable: false),
                    SourceId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    Channel = table.Column<byte>(nullable: false),
                    Status = table.Column<byte>(nullable: false),
                    PriceType = table.Column<byte>(nullable: false),
                    ItemPrice = table.Column<decimal>(nullable: false),
                    ItemVatAmount = table.Column<decimal>(nullable: false),
                    ItemTotalAmount = table.Column<decimal>(nullable: false),
                    ItemSubTotalAmountWithCommission = table.Column<decimal>(nullable: false),
                    ItemVatAmountWithCommission = table.Column<decimal>(nullable: false),
                    ItemTotalAmountWithCommission = table.Column<decimal>(nullable: false),
                    TotalAmount = table.Column<decimal>(nullable: false),
                    SubTotalAmount = table.Column<decimal>(nullable: false),
                    VatAmount = table.Column<decimal>(nullable: false),
                    TotalAmountWithCommission = table.Column<decimal>(nullable: false),
                    SubTotalAmountWithCommission = table.Column<decimal>(nullable: false),
                    VatAmountWithCommission = table.Column<decimal>(nullable: false),
                    TaxVat = table.Column<decimal>(nullable: false),
                    CommissionType = table.Column<byte>(nullable: false),
                    ItemCommissionAmount = table.Column<decimal>(nullable: false),
                    CommissionPercentageOrAddValue = table.Column<decimal>(nullable: false),
                    CommissionAmount = table.Column<decimal>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    RejectedReason = table.Column<string>(nullable: true),
                    IsView = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceOffers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PriceOffers_PriceOffers_ParentId",
                        column: x => x.ParentId,
                        principalTable: "PriceOffers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PriceOffers_ShippingRequests_ShippingRequestId",
                        column: x => x.ShippingRequestId,
                        principalTable: "ShippingRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PriceOffers_AbpTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PriceOfferDetails",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PriceOfferId = table.Column<long>(nullable: false),
                    SourceId = table.Column<long>(nullable: true),
                    ItemPrice = table.Column<decimal>(nullable: false),
                    ItemVatAmount = table.Column<decimal>(nullable: false),
                    ItemTotalAmount = table.Column<decimal>(nullable: false),
                    ItemSubTotalAmountWithCommission = table.Column<decimal>(nullable: false),
                    ItemVatAmountWithCommission = table.Column<decimal>(nullable: false),
                    ItemTotalAmountWithCommission = table.Column<decimal>(nullable: false),
                    TotalAmount = table.Column<decimal>(nullable: false),
                    SubTotalAmount = table.Column<decimal>(nullable: false),
                    VatAmount = table.Column<decimal>(nullable: false),
                    TotalAmountWithCommission = table.Column<decimal>(nullable: false),
                    SubTotalAmountWithCommission = table.Column<decimal>(nullable: false),
                    VatAmountWithCommission = table.Column<decimal>(nullable: false),
                    ItemCommissionAmount = table.Column<decimal>(nullable: false),
                    CommissionAmount = table.Column<decimal>(nullable: false),
                    CommissionType = table.Column<byte>(nullable: false),
                    CommissionPercentageOrAddValue = table.Column<decimal>(nullable: false),
                    Quantity = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceOfferDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PriceOfferDetails_PriceOffers_PriceOfferId",
                        column: x => x.PriceOfferId,
                        principalTable: "PriceOffers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestVasesPricing_PriceOfferId",
                table: "ShippingRequestVasesPricing",
                column: "PriceOfferId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceOfferDetails_PriceOfferId",
                table: "PriceOfferDetails",
                column: "PriceOfferId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceOffers_ParentId",
                table: "PriceOffers",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceOffers_ShippingRequestId",
                table: "PriceOffers",
                column: "ShippingRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceOffers_TenantId",
                table: "PriceOffers",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequestVasesPricing_PriceOffers_PriceOfferId",
                table: "ShippingRequestVasesPricing",
                column: "PriceOfferId",
                principalTable: "PriceOffers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestVasesPricing_PriceOffers_PriceOfferId",
                table: "ShippingRequestVasesPricing");

            migrationBuilder.DropTable(
                name: "PriceOfferDetails");

            migrationBuilder.DropTable(
                name: "PriceOffers");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequestVasesPricing_PriceOfferId",
                table: "ShippingRequestVasesPricing");

            migrationBuilder.DropColumn(
                name: "PriceOfferId",
                table: "ShippingRequestVasesPricing");
        }
    }
}
