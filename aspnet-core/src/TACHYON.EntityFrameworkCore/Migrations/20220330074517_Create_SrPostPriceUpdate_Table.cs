using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class Create_SrPostPriceUpdate_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShippingRequestPostPriceUpdates",
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
                    ShippingRequestId = table.Column<long>(nullable: false),
                    Action = table.Column<int>(nullable: false),
                    RejectionReason = table.Column<string>(nullable: true),
                    UpdateChanges = table.Column<string>(nullable: true),
                    IsApplied = table.Column<bool>(nullable: false),
                    PriceOfferId = table.Column<long>(nullable: true),
                    OfferStatus = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingRequestPostPriceUpdates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingRequestPostPriceUpdates_PriceOffers_PriceOfferId",
                        column: x => x.PriceOfferId,
                        principalTable: "PriceOffers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShippingRequestPostPriceUpdates_ShippingRequests_ShippingRequestId",
                        column: x => x.ShippingRequestId,
                        principalTable: "ShippingRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestPostPriceUpdates_PriceOfferId",
                table: "ShippingRequestPostPriceUpdates",
                column: "PriceOfferId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestPostPriceUpdates_ShippingRequestId",
                table: "ShippingRequestPostPriceUpdates",
                column: "ShippingRequestId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShippingRequestPostPriceUpdates");
        }
    }
}
