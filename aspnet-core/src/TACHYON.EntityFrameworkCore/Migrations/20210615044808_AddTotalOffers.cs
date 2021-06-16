using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class AddTotalOffers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShippingRequestVasesPricing");

            migrationBuilder.DropTable(
                name: "ShippingRequestPricings");

            migrationBuilder.AddColumn<int>(
                name: "TotalOffers",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalOffers",
                table: "ShippingRequests");

            migrationBuilder.CreateTable(
                name: "ShippingRequestPricings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Channel = table.Column<byte>(type: "tinyint", nullable: false),
                    CommissionAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CommissionPercentageOrAddValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CommissionType = table.Column<byte>(type: "tinyint", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsView = table.Column<bool>(type: "bit", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    ParentId = table.Column<long>(type: "bigint", nullable: true),
                    ReferenceNumber = table.Column<long>(type: "bigint", nullable: true),
                    RejectedReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShippingRequestId = table.Column<long>(type: "bigint", nullable: false),
                    SourceId = table.Column<long>(type: "bigint", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SubTotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SubTotalAmountWithCommission = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaxVat = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalAmountWithCommission = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TripCommissionAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TripPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TripSubTotalAmountWithCommission = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TripTotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TripTotalAmountWithCommission = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TripVatAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TripVatAmountWithCommission = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VatAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VatAmountWithCommission = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingRequestPricings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingRequestPricings_ShippingRequestPricings_ParentId",
                        column: x => x.ParentId,
                        principalTable: "ShippingRequestPricings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShippingRequestPricings_ShippingRequests_ShippingRequestId",
                        column: x => x.ShippingRequestId,
                        principalTable: "ShippingRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShippingRequestPricings_AbpTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShippingRequestVasesPricing",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CommissionAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CommissionPercentageOrAddValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CommissionType = table.Column<byte>(type: "tinyint", nullable: false),
                    ShippingRequestPricingId = table.Column<long>(type: "bigint", nullable: false),
                    SubTotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SubTotalAmountWithCommission = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalAmountWithCommission = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VasCommissionAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VasId = table.Column<long>(type: "bigint", nullable: false),
                    VasPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VasSubTotalAmountWithCommission = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VasTotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VasTotalAmountWithCommission = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VasVatAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VasVatAmountWithCommission = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VatAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VatAmountWithCommission = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingRequestVasesPricing", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingRequestVasesPricing_ShippingRequestPricings_ShippingRequestPricingId",
                        column: x => x.ShippingRequestPricingId,
                        principalTable: "ShippingRequestPricings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShippingRequestVasesPricing_ShippingRequestVases_VasId",
                        column: x => x.VasId,
                        principalTable: "ShippingRequestVases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestPricings_ParentId",
                table: "ShippingRequestPricings",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestPricings_ShippingRequestId",
                table: "ShippingRequestPricings",
                column: "ShippingRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestPricings_TenantId",
                table: "ShippingRequestPricings",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestVasesPricing_ShippingRequestPricingId",
                table: "ShippingRequestVasesPricing",
                column: "ShippingRequestPricingId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestVasesPricing_VasId",
                table: "ShippingRequestVasesPricing",
                column: "VasId");
        }
    }
}
