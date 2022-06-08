using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class Create_ShippingRequestUpdates_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShippingRequestUpdates",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    ShippingRequestId = table.Column<long>(nullable: true),
                    EntityLogId = table.Column<Guid>(nullable: false),
                    PriceOfferId = table.Column<long>(nullable: false),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingRequestUpdates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingRequestUpdates_EntityHistoryLog_EntityLogId",
                        column: x => x.EntityLogId,
                        principalTable: "EntityHistoryLog",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShippingRequestUpdates_PriceOffers_PriceOfferId",
                        column: x => x.PriceOfferId,
                        principalTable: "PriceOffers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShippingRequestUpdates_ShippingRequests_ShippingRequestId",
                        column: x => x.ShippingRequestId,
                        principalTable: "ShippingRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestUpdates_EntityLogId",
                table: "ShippingRequestUpdates",
                column: "EntityLogId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestUpdates_PriceOfferId",
                table: "ShippingRequestUpdates",
                column: "PriceOfferId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestUpdates_ShippingRequestId",
                table: "ShippingRequestUpdates",
                column: "ShippingRequestId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShippingRequestUpdates");
        }
    }
}
