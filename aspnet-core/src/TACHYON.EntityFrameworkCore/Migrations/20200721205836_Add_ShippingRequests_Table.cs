using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class Add_ShippingRequests_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShippingRequests",
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
                    TenantId = table.Column<int>(nullable: false),
                    Vas = table.Column<decimal>(nullable: false),
                    TrucksTypeId = table.Column<Guid>(nullable: true),
                    TrailerTypeId = table.Column<int>(nullable: true),
                    GoodsDetailId = table.Column<long>(nullable: true),
                    RouteId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingRequests_GoodsDetails_GoodsDetailId",
                        column: x => x.GoodsDetailId,
                        principalTable: "GoodsDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShippingRequests_Routes_RouteId",
                        column: x => x.RouteId,
                        principalTable: "Routes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShippingRequests_TrailerTypes_TrailerTypeId",
                        column: x => x.TrailerTypeId,
                        principalTable: "TrailerTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShippingRequests_TrucksTypes_TrucksTypeId",
                        column: x => x.TrucksTypeId,
                        principalTable: "TrucksTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequests_GoodsDetailId",
                table: "ShippingRequests",
                column: "GoodsDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequests_RouteId",
                table: "ShippingRequests",
                column: "RouteId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequests_TenantId",
                table: "ShippingRequests",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequests_TrailerTypeId",
                table: "ShippingRequests",
                column: "TrailerTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequests_TrucksTypeId",
                table: "ShippingRequests",
                column: "TrucksTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShippingRequests");
        }
    }
}
