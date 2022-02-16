using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace TACHYON.Migrations
{
    public partial class add_direct_request_to_bid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "BidNormalPricePackageId",
                table: "ShippingRequestDirectRequests",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TachyonMSRequestPrice",
                table: "NormalPricePackages",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<decimal>(
                name: "PricePerExtraDrop",
                table: "NormalPricePackages",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "MarcketPlaceRequestPrice",
                table: "NormalPricePackages",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<decimal>(
                name: "DirectRequestPrice",
                table: "NormalPricePackages",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");


            migrationBuilder.CreateTable(
                name: "BidNormalPricePackages",
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
                    SourceId = table.Column<long>(nullable: true),
                    PriceType = table.Column<byte>(nullable: false),
                    ItemPrice = table.Column<decimal>(nullable: false),
                    ItemVatAmount = table.Column<decimal>(nullable: false),
                    ItemTotalAmount = table.Column<decimal>(nullable: false),
                    ItemSubTotalAmountWithCommission = table.Column<decimal>(nullable: false),
                    ItemVatAmountWithCommission = table.Column<decimal>(nullable: false),
                    ItemTotalAmountWithCommission = table.Column<decimal>(nullable: false),
                    ItemsTotalPricePreCommissionPreVat = table.Column<decimal>(nullable: false),
                    TotalAmount = table.Column<decimal>(nullable: false),
                    SubTotalAmount = table.Column<decimal>(nullable: false),
                    VatAmount = table.Column<decimal>(nullable: false),
                    TotalAmountWithCommission = table.Column<decimal>(nullable: false),
                    SubTotalAmountWithCommission = table.Column<decimal>(nullable: false),
                    VatAmountWithCommission = table.Column<decimal>(nullable: false),
                    CommissionType = table.Column<byte>(nullable: false),
                    ItemCommissionAmount = table.Column<decimal>(nullable: false),
                    CommissionPercentageOrAddValue = table.Column<decimal>(nullable: false),
                    CommissionAmount = table.Column<decimal>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    ItemsTotalVatAmountPreCommission = table.Column<decimal>(nullable: false),
                    ItemsTotalCommission = table.Column<decimal>(nullable: false),
                    ItemsTotalPricePostCommissionPreVat = table.Column<decimal>(nullable: false),
                    ItemsTotalVatPostCommission = table.Column<decimal>(nullable: false),
                    DetailsTotalPricePreCommissionPreVat = table.Column<decimal>(nullable: false),
                    DetailsTotalVatAmountPreCommission = table.Column<decimal>(nullable: false),
                    DetailsTotalCommission = table.Column<decimal>(nullable: false),
                    DetailsTotalPricePostCommissionPreVat = table.Column<decimal>(nullable: false),
                    DetailsTotalVatPostCommission = table.Column<decimal>(nullable: false),
                    TaxVat = table.Column<decimal>(nullable: false),
                    PricePackageId = table.Column<string>(nullable: true),
                    DisplayName = table.Column<string>(maxLength: 100, nullable: false),
                    TenantId = table.Column<int>(nullable: false),
                    TransportTypeId = table.Column<int>(nullable: false),
                    TrucksTypeId = table.Column<long>(nullable: false),
                    NormalPricePackageId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BidNormalPricePackages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BidNormalPricePackages_NormalPricePackages_NormalPricePackageId",
                        column: x => x.NormalPricePackageId,
                        principalTable: "NormalPricePackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BidNormalPricePackages_AbpTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BidNormalPricePackages_TransportTypes_TransportTypeId",
                        column: x => x.TransportTypeId,
                        principalTable: "TransportTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BidNormalPricePackages_TrucksTypes_TrucksTypeId",
                        column: x => x.TrucksTypeId,
                        principalTable: "TrucksTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BidPricePackageDetails",
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
                    SourceId = table.Column<long>(nullable: true),
                    PriceType = table.Column<byte>(nullable: false),
                    ItemPrice = table.Column<decimal>(nullable: false),
                    ItemVatAmount = table.Column<decimal>(nullable: false),
                    ItemTotalAmount = table.Column<decimal>(nullable: false),
                    ItemSubTotalAmountWithCommission = table.Column<decimal>(nullable: false),
                    ItemVatAmountWithCommission = table.Column<decimal>(nullable: false),
                    ItemTotalAmountWithCommission = table.Column<decimal>(nullable: false),
                    ItemsTotalPricePreCommissionPreVat = table.Column<decimal>(nullable: false),
                    TotalAmount = table.Column<decimal>(nullable: false),
                    SubTotalAmount = table.Column<decimal>(nullable: false),
                    VatAmount = table.Column<decimal>(nullable: false),
                    TotalAmountWithCommission = table.Column<decimal>(nullable: false),
                    SubTotalAmountWithCommission = table.Column<decimal>(nullable: false),
                    VatAmountWithCommission = table.Column<decimal>(nullable: false),
                    CommissionType = table.Column<byte>(nullable: false),
                    ItemCommissionAmount = table.Column<decimal>(nullable: false),
                    CommissionPercentageOrAddValue = table.Column<decimal>(nullable: false),
                    CommissionAmount = table.Column<decimal>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    BidNormalPricePackageId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BidPricePackageDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BidPricePackageDetails_BidNormalPricePackages_BidNormalPricePackageId",
                        column: x => x.BidNormalPricePackageId,
                        principalTable: "BidNormalPricePackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestDirectRequests_BidNormalPricePackageId",
                table: "ShippingRequestDirectRequests",
                column: "BidNormalPricePackageId");

            migrationBuilder.CreateIndex(
                name: "IX_BidNormalPricePackages_NormalPricePackageId",
                table: "BidNormalPricePackages",
                column: "NormalPricePackageId");

            migrationBuilder.CreateIndex(
                name: "IX_BidNormalPricePackages_TenantId",
                table: "BidNormalPricePackages",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_BidNormalPricePackages_TransportTypeId",
                table: "BidNormalPricePackages",
                column: "TransportTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BidNormalPricePackages_TrucksTypeId",
                table: "BidNormalPricePackages",
                column: "TrucksTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BidPricePackageDetails_BidNormalPricePackageId",
                table: "BidPricePackageDetails",
                column: "BidNormalPricePackageId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequestDirectRequests_BidNormalPricePackages_BidNormalPricePackageId",
                table: "ShippingRequestDirectRequests",
                column: "BidNormalPricePackageId",
                principalTable: "BidNormalPricePackages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestDirectRequests_BidNormalPricePackages_BidNormalPricePackageId",
                table: "ShippingRequestDirectRequests");

            migrationBuilder.DropTable(
                name: "BidPricePackageDetails");

            migrationBuilder.DropTable(
                name: "BidNormalPricePackages");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequestDirectRequests_BidNormalPricePackageId",
                table: "ShippingRequestDirectRequests");

            migrationBuilder.DropColumn(
                name: "BidNormalPricePackageId",
                table: "ShippingRequestDirectRequests");


            migrationBuilder.AlterColumn<float>(
                name: "TachyonMSRequestPrice",
                table: "NormalPricePackages",
                type: "real",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<float>(
                name: "PricePerExtraDrop",
                table: "NormalPricePackages",
                type: "real",
                nullable: true,
                oldClrType: typeof(decimal),
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "MarcketPlaceRequestPrice",
                table: "NormalPricePackages",
                type: "real",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<float>(
                name: "DirectRequestPrice",
                table: "NormalPricePackages",
                type: "real",
                nullable: false,
                oldClrType: typeof(decimal));
        }
    }
}