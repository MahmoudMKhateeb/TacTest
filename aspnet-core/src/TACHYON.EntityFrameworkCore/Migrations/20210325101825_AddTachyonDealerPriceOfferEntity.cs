using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class AddTachyonDealerPriceOfferEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CarrierPrice",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "CarrierPriceType",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TachyonDealerCommissionValue",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TachyonDealerMinValueCommission",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TachyonDealerPercentCommission",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TachyonDealerProfit",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalCommission",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TachyonPriceOffers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    ShippingRequestId = table.Column<long>(nullable: false),
                    OfferedPrice = table.Column<decimal>(nullable: false),
                    ShippingRequestBidId = table.Column<long>(nullable: true),
                    OfferStatus = table.Column<byte>(nullable: false),
                    RejectedReason = table.Column<string>(nullable: true),
                    PriceType = table.Column<byte>(nullable: false),
                    TachyonDealerPercentCommission = table.Column<decimal>(nullable: false),
                    TachyonDealerCommissionValue = table.Column<decimal>(nullable: false),
                    TachyonDealerMinCommissionValue = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TachyonPriceOffers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TachyonPriceOffers_ShippingRequestBids_ShippingRequestBidId",
                        column: x => x.ShippingRequestBidId,
                        principalTable: "ShippingRequestBids",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TachyonPriceOffers_ShippingRequests_ShippingRequestId",
                        column: x => x.ShippingRequestId,
                        principalTable: "ShippingRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TachyonPriceOffers_ShippingRequestBidId",
                table: "TachyonPriceOffers",
                column: "ShippingRequestBidId");

            migrationBuilder.CreateIndex(
                name: "IX_TachyonPriceOffers_ShippingRequestId",
                table: "TachyonPriceOffers",
                column: "ShippingRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_TachyonPriceOffers_TenantId",
                table: "TachyonPriceOffers",
                column: "TenantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TachyonPriceOffers");

            migrationBuilder.DropColumn(
                name: "CarrierPrice",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "CarrierPriceType",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "TachyonDealerCommissionValue",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "TachyonDealerMinValueCommission",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "TachyonDealerPercentCommission",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "TachyonDealerProfit",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "TotalCommission",
                table: "ShippingRequests");
        }
    }
}
