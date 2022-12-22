using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class release_2_4_25 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TmsPriceOfferPackageOffers");
        }
    }
}
