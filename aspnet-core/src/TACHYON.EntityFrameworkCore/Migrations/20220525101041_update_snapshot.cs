using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class update_snapshot : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "DriverLicenseTypeTranslations");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "DriverLicenseTypes");

            migrationBuilder.AddColumn<long>(
                name: "PricePackageOfferId",
                table: "ShippingRequestDirectRequests",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "NormalPricePackages",
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
                    PricePackageId = table.Column<string>(nullable: true),
                    DisplayName = table.Column<string>(maxLength: 100, nullable: false),
                    TenantId = table.Column<int>(nullable: false),
                    TransportTypeId = table.Column<int>(nullable: false),
                    TrucksTypeId = table.Column<long>(nullable: false),
                    DirectRequestPrice = table.Column<decimal>(nullable: false),
                    MarcketPlaceRequestPrice = table.Column<decimal>(nullable: false),
                    TachyonMSRequestPrice = table.Column<decimal>(nullable: false),
                    PricePerExtraDrop = table.Column<decimal>(nullable: true),
                    IsMultiDrop = table.Column<bool>(nullable: false),
                    OriginCityId = table.Column<int>(nullable: true),
                    DestinationCityId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NormalPricePackages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NormalPricePackages_Cities_DestinationCityId",
                        column: x => x.DestinationCityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NormalPricePackages_Cities_OriginCityId",
                        column: x => x.OriginCityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NormalPricePackages_AbpTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NormalPricePackages_TransportTypes_TransportTypeId",
                        column: x => x.TransportTypeId,
                        principalTable: "TransportTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NormalPricePackages_TrucksTypes_TrucksTypeId",
                        column: x => x.TrucksTypeId,
                        principalTable: "TrucksTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PricePackageOffers",
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
                    OriginCityId = table.Column<int>(nullable: true),
                    DestinationCityId = table.Column<int>(nullable: true),
                    NormalPricePackageId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PricePackageOffers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PricePackageOffers_Cities_DestinationCityId",
                        column: x => x.DestinationCityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PricePackageOffers_NormalPricePackages_NormalPricePackageId",
                        column: x => x.NormalPricePackageId,
                        principalTable: "NormalPricePackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PricePackageOffers_Cities_OriginCityId",
                        column: x => x.OriginCityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PricePackageOffers_AbpTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PricePackageOffers_TransportTypes_TransportTypeId",
                        column: x => x.TransportTypeId,
                        principalTable: "TransportTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PricePackageOffers_TrucksTypes_TrucksTypeId",
                        column: x => x.TrucksTypeId,
                        principalTable: "TrucksTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                name: "IX_ShippingRequestDirectRequests_PricePackageOfferId",
                table: "ShippingRequestDirectRequests",
                column: "PricePackageOfferId");

            migrationBuilder.CreateIndex(
                name: "IX_NormalPricePackages_DestinationCityId",
                table: "NormalPricePackages",
                column: "DestinationCityId");

            migrationBuilder.CreateIndex(
                name: "IX_NormalPricePackages_OriginCityId",
                table: "NormalPricePackages",
                column: "OriginCityId");

            migrationBuilder.CreateIndex(
                name: "IX_NormalPricePackages_TenantId",
                table: "NormalPricePackages",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_NormalPricePackages_TransportTypeId",
                table: "NormalPricePackages",
                column: "TransportTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_NormalPricePackages_TrucksTypeId",
                table: "NormalPricePackages",
                column: "TrucksTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PricePackageOfferItems_BidNormalPricePackageId",
                table: "PricePackageOfferItems",
                column: "BidNormalPricePackageId");

            migrationBuilder.CreateIndex(
                name: "IX_PricePackageOffers_DestinationCityId",
                table: "PricePackageOffers",
                column: "DestinationCityId");

            migrationBuilder.CreateIndex(
                name: "IX_PricePackageOffers_NormalPricePackageId",
                table: "PricePackageOffers",
                column: "NormalPricePackageId");

            migrationBuilder.CreateIndex(
                name: "IX_PricePackageOffers_OriginCityId",
                table: "PricePackageOffers",
                column: "OriginCityId");

            migrationBuilder.CreateIndex(
                name: "IX_PricePackageOffers_TenantId",
                table: "PricePackageOffers",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PricePackageOffers_TransportTypeId",
                table: "PricePackageOffers",
                column: "TransportTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PricePackageOffers_TrucksTypeId",
                table: "PricePackageOffers",
                column: "TrucksTypeId");

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
                name: "FK_ShippingRequestDirectRequests_PricePackageOffers_PricePackageOfferId",
                table: "ShippingRequestDirectRequests");

            migrationBuilder.DropTable(
                name: "PricePackageOfferItems");

            migrationBuilder.DropTable(
                name: "PricePackageOffers");

            migrationBuilder.DropTable(
                name: "NormalPricePackages");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequestDirectRequests_PricePackageOfferId",
                table: "ShippingRequestDirectRequests");

            migrationBuilder.DropColumn(
                name: "PricePackageOfferId",
                table: "ShippingRequestDirectRequests");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "DriverLicenseTypeTranslations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "DriverLicenseTypes",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
