using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class AddCommissionPercentageOrAddValue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommissionPercentage",
                table: "ShippingRequestPricings");

            migrationBuilder.DropColumn(
                name: "CommissionValue",
                table: "ShippingRequestPricings");

            migrationBuilder.AddColumn<decimal>(
                name: "CommissionPercentageOrAddValue",
                table: "ShippingRequestPricings",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "ShippingRequestVasesPricing",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShippingRequestPricingId = table.Column<long>(nullable: false),
                    VasId = table.Column<long>(nullable: false),
                    VasPrice = table.Column<decimal>(nullable: false),
                    VasVatAmount = table.Column<decimal>(nullable: false),
                    VasTotalAmount = table.Column<decimal>(nullable: false),
                    VasSubTotalAmountWithCommission = table.Column<decimal>(nullable: false),
                    VasVatAmountWithCommission = table.Column<decimal>(nullable: false),
                    VasTotalAmountWithCommission = table.Column<decimal>(nullable: false),
                    TotalAmount = table.Column<decimal>(nullable: false),
                    SubTotalAmount = table.Column<decimal>(nullable: false),
                    VatAmount = table.Column<decimal>(nullable: false),
                    TotalAmountWithCommission = table.Column<decimal>(nullable: false),
                    SubTotalAmountWithCommission = table.Column<decimal>(nullable: false),
                    VatAmountWithCommission = table.Column<decimal>(nullable: false),
                    VasCommissionAmount = table.Column<decimal>(nullable: false),
                    CommissionAmount = table.Column<decimal>(nullable: false)
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
                name: "IX_ShippingRequestVasesPricing_ShippingRequestPricingId",
                table: "ShippingRequestVasesPricing",
                column: "ShippingRequestPricingId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestVasesPricing_VasId",
                table: "ShippingRequestVasesPricing",
                column: "VasId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShippingRequestVasesPricing");

            migrationBuilder.DropColumn(
                name: "CommissionPercentageOrAddValue",
                table: "ShippingRequestPricings");

            migrationBuilder.AddColumn<decimal>(
                name: "CommissionPercentage",
                table: "ShippingRequestPricings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CommissionValue",
                table: "ShippingRequestPricings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
