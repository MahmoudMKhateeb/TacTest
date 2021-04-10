using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class ShippingRequestsCarrierDirectPricing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShippingRequestsCarrierDirectPricing",
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
                    RequestId = table.Column<long>(nullable: false),
                    Price = table.Column<decimal>(nullable: true),
                    FinalPrice = table.Column<decimal>(nullable: true),
                    PercentageComission = table.Column<decimal>(nullable: true),
                    ValueComission = table.Column<decimal>(nullable: true),
                    MinValueComission = table.Column<decimal>(nullable: true),
                    TaxVat = table.Column<decimal>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    RejetcReason = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingRequestsCarrierDirectPricing", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingRequestsCarrierDirectPricing_ShippingRequests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "ShippingRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestsCarrierDirectPricing_RequestId",
                table: "ShippingRequestsCarrierDirectPricing",
                column: "RequestId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShippingRequestsCarrierDirectPricing");
        }
    }
}
