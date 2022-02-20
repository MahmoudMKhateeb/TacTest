using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace TACHYON.Migrations
{
    public partial class change_tables_names2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BidNormalPricePackages_NormalPricePackages_NormalPricePackageId",
                table: "BidNormalPricePackages");

            migrationBuilder.DropForeignKey(
                name: "FK_BidNormalPricePackages_AbpTenants_TenantId",
                table: "BidNormalPricePackages");

            migrationBuilder.DropForeignKey(
                name: "FK_BidNormalPricePackages_TransportTypes_TransportTypeId",
                table: "BidNormalPricePackages");

            migrationBuilder.DropForeignKey(
                name: "FK_BidNormalPricePackages_TrucksTypes_TrucksTypeId",
                table: "BidNormalPricePackages");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestDirectRequests_BidNormalPricePackages_PricePackageOfferId",
                table: "ShippingRequestDirectRequests");

            migrationBuilder.DropTable(
                name: "BidPricePackageDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BidNormalPricePackages",
                table: "BidNormalPricePackages");

            migrationBuilder.RenameTable(
                name: "BidNormalPricePackages",
                newName: "PricePackageOffers");

            migrationBuilder.RenameIndex(
                name: "IX_BidNormalPricePackages_TrucksTypeId",
                table: "PricePackageOffers",
                newName: "IX_PricePackageOffers_TrucksTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_BidNormalPricePackages_TransportTypeId",
                table: "PricePackageOffers",
                newName: "IX_PricePackageOffers_TransportTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_BidNormalPricePackages_TenantId",
                table: "PricePackageOffers",
                newName: "IX_PricePackageOffers_TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_BidNormalPricePackages_NormalPricePackageId",
                table: "PricePackageOffers",
                newName: "IX_PricePackageOffers_NormalPricePackageId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PricePackageOffers",
                table: "PricePackageOffers",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "PricePackageOfferItems",
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
                    table.PrimaryKey("PK_PricePackageOfferItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PricePackageOfferItems_PricePackageOffers_BidNormalPricePackageId",
                        column: x => x.BidNormalPricePackageId,
                        principalTable: "PricePackageOffers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PricePackageOfferItems_BidNormalPricePackageId",
                table: "PricePackageOfferItems",
                column: "BidNormalPricePackageId");

            migrationBuilder.AddForeignKey(
                name: "FK_PricePackageOffers_NormalPricePackages_NormalPricePackageId",
                table: "PricePackageOffers",
                column: "NormalPricePackageId",
                principalTable: "NormalPricePackages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PricePackageOffers_AbpTenants_TenantId",
                table: "PricePackageOffers",
                column: "TenantId",
                principalTable: "AbpTenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PricePackageOffers_TransportTypes_TransportTypeId",
                table: "PricePackageOffers",
                column: "TransportTypeId",
                principalTable: "TransportTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PricePackageOffers_TrucksTypes_TrucksTypeId",
                table: "PricePackageOffers",
                column: "TrucksTypeId",
                principalTable: "TrucksTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequestDirectRequests_PricePackageOffers_PricePackageOfferId",
                table: "ShippingRequestDirectRequests",
                column: "PricePackageOfferId",
                principalTable: "PricePackageOffers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PricePackageOffers_NormalPricePackages_NormalPricePackageId",
                table: "PricePackageOffers");

            migrationBuilder.DropForeignKey(
                name: "FK_PricePackageOffers_AbpTenants_TenantId",
                table: "PricePackageOffers");

            migrationBuilder.DropForeignKey(
                name: "FK_PricePackageOffers_TransportTypes_TransportTypeId",
                table: "PricePackageOffers");

            migrationBuilder.DropForeignKey(
                name: "FK_PricePackageOffers_TrucksTypes_TrucksTypeId",
                table: "PricePackageOffers");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestDirectRequests_PricePackageOffers_PricePackageOfferId",
                table: "ShippingRequestDirectRequests");

            migrationBuilder.DropTable(
                name: "PricePackageOfferItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PricePackageOffers",
                table: "PricePackageOffers");

            migrationBuilder.RenameTable(
                name: "PricePackageOffers",
                newName: "BidNormalPricePackages");

            migrationBuilder.RenameIndex(
                name: "IX_PricePackageOffers_TrucksTypeId",
                table: "BidNormalPricePackages",
                newName: "IX_BidNormalPricePackages_TrucksTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_PricePackageOffers_TransportTypeId",
                table: "BidNormalPricePackages",
                newName: "IX_BidNormalPricePackages_TransportTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_PricePackageOffers_TenantId",
                table: "BidNormalPricePackages",
                newName: "IX_BidNormalPricePackages_TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_PricePackageOffers_NormalPricePackageId",
                table: "BidNormalPricePackages",
                newName: "IX_BidNormalPricePackages_NormalPricePackageId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BidNormalPricePackages",
                table: "BidNormalPricePackages",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "BidPricePackageDetails",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BidNormalPricePackageId = table.Column<long>(type: "bigint", nullable: false),
                    CommissionAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CommissionPercentageOrAddValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CommissionType = table.Column<byte>(type: "tinyint", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ItemCommissionAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ItemPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ItemSubTotalAmountWithCommission = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ItemTotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ItemTotalAmountWithCommission = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ItemVatAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ItemVatAmountWithCommission = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ItemsTotalPricePreCommissionPreVat = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    PriceType = table.Column<byte>(type: "tinyint", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    SourceId = table.Column<long>(type: "bigint", nullable: true),
                    SubTotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SubTotalAmountWithCommission = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalAmountWithCommission = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VatAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VatAmountWithCommission = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
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
                name: "IX_BidPricePackageDetails_BidNormalPricePackageId",
                table: "BidPricePackageDetails",
                column: "BidNormalPricePackageId");

            migrationBuilder.AddForeignKey(
                name: "FK_BidNormalPricePackages_NormalPricePackages_NormalPricePackageId",
                table: "BidNormalPricePackages",
                column: "NormalPricePackageId",
                principalTable: "NormalPricePackages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BidNormalPricePackages_AbpTenants_TenantId",
                table: "BidNormalPricePackages",
                column: "TenantId",
                principalTable: "AbpTenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BidNormalPricePackages_TransportTypes_TransportTypeId",
                table: "BidNormalPricePackages",
                column: "TransportTypeId",
                principalTable: "TransportTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BidNormalPricePackages_TrucksTypes_TrucksTypeId",
                table: "BidNormalPricePackages",
                column: "TrucksTypeId",
                principalTable: "TrucksTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequestDirectRequests_BidNormalPricePackages_PricePackageOfferId",
                table: "ShippingRequestDirectRequests",
                column: "PricePackageOfferId",
                principalTable: "BidNormalPricePackages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}