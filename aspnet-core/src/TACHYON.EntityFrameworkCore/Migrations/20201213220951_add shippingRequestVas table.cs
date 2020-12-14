using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class addshippingRequestVastable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShippingRequestVases",
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
                    DefualtPrice = table.Column<long>(nullable: true),
                    ActualPrice = table.Column<long>(nullable: true),
                    RequestMaxAmount = table.Column<int>(nullable: false),
                    RequestMaxCount = table.Column<int>(nullable: false),
                    VasId = table.Column<int>(nullable: false),
                    ShippingRequestId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingRequestVases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingRequestVases_ShippingRequests_ShippingRequestId",
                        column: x => x.ShippingRequestId,
                        principalTable: "ShippingRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShippingRequestVases_Vases_VasId",
                        column: x => x.VasId,
                        principalTable: "Vases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestVases_ShippingRequestId",
                table: "ShippingRequestVases",
                column: "ShippingRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestVases_TenantId",
                table: "ShippingRequestVases",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestVases_VasId",
                table: "ShippingRequestVases",
                column: "VasId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShippingRequestVases");
        }
    }
}
