using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

namespace TACHYON.Migrations
{
    public partial class Production_v1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentFiles_DocumentTypes_DocumentTypeId",
                table: "DocumentFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_GoodsDetails_GoodCategories_GoodCategoryId",
                table: "GoodsDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Routes_Cities_DestinationCityId",
                table: "Routes");

            migrationBuilder.DropForeignKey(
                name: "FK_Routes_Cities_OriginCityId",
                table: "Routes");

            migrationBuilder.DropForeignKey(
                name: "FK_RoutSteps_AbpUsers_AssignedDriverUserId",
                table: "RoutSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_RoutSteps_Trailers_AssignedTrailerId",
                table: "RoutSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_RoutSteps_Trucks_AssignedTruckId",
                table: "RoutSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_RoutSteps_Cities_DestinationCityId",
                table: "RoutSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_RoutSteps_Facilities_DestinationFacilityId",
                table: "RoutSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_RoutSteps_GoodsDetails_GoodsDetailId",
                table: "RoutSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_RoutSteps_Cities_OriginCityId",
                table: "RoutSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_RoutSteps_PickingTypes_PickingTypeId",
                table: "RoutSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_RoutSteps_ShippingRequests_ShippingRequestId",
                table: "RoutSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_RoutSteps_Facilities_SourceFacilityId",
                table: "RoutSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_RoutSteps_TrailerTypes_TrailerTypeId",
                table: "RoutSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_RoutSteps_TrucksTypes_TrucksTypeId",
                table: "RoutSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_Trailers_AssignedTrailerId",
                table: "ShippingRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_GoodCategories_GoodCategoryId",
                table: "ShippingRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_Routes_RouteId",
                table: "ShippingRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_ShippingRequestBidStatuses_ShippingRequestBidStatusId",
                table: "ShippingRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_ShippingRequestStatuses_ShippingRequestStatusId",
                table: "ShippingRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_TrucksTypes_TrucksTypeId",
                table: "ShippingRequests");

            migrationBuilder.DropTable(
                name: "PickingTypes");

            migrationBuilder.DropTable(
                name: "ShippingRequestBidStatuses");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequestVases_TenantId",
                table: "ShippingRequestVases");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequests_AssignedTrailerId",
                table: "ShippingRequests");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequests_RouteId",
                table: "ShippingRequests");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequests_ShippingRequestBidStatusId",
                table: "ShippingRequests");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequests_ShippingRequestStatusId",
                table: "ShippingRequests");

            migrationBuilder.DropIndex(
                name: "IX_RoutSteps_AssignedTrailerId",
                table: "RoutSteps");

            migrationBuilder.DropIndex(
                name: "IX_RoutSteps_DestinationCityId",
                table: "RoutSteps");

            migrationBuilder.DropIndex(
                name: "IX_RoutSteps_DestinationFacilityId",
                table: "RoutSteps");

            migrationBuilder.DropIndex(
                name: "IX_RoutSteps_GoodsDetailId",
                table: "RoutSteps");

            migrationBuilder.DropIndex(
                name: "IX_RoutSteps_OriginCityId",
                table: "RoutSteps");

            migrationBuilder.DropIndex(
                name: "IX_RoutSteps_PickingTypeId",
                table: "RoutSteps");

            migrationBuilder.DropIndex(
                name: "IX_RoutSteps_SourceFacilityId",
                table: "RoutSteps");

            migrationBuilder.DropIndex(
                name: "IX_RoutSteps_TrailerTypeId",
                table: "RoutSteps");

            migrationBuilder.DropIndex(
                name: "IX_RoutSteps_TrucksTypeId",
                table: "RoutSteps");

            migrationBuilder.DropIndex(
                name: "IX_Routes_TenantId",
                table: "Routes");

            migrationBuilder.DropIndex(
                name: "IX_GoodsDetails_TenantId",
                table: "GoodsDetails");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "Vases");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "TrucksTypes");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "ShippingRequestVases");

            migrationBuilder.DropColumn(
                name: "AssignedTrailerId",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "IsRejected",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "RouteId",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "ShippingRequestBidStatusId",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "ShippingRequestStatusId",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "StageOneFinish",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "StageThreeFinish",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "StageTowFinish",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "AssignedTrailerId",
                table: "RoutSteps");

            migrationBuilder.DropColumn(
                name: "DestinationCityId",
                table: "RoutSteps");

            migrationBuilder.DropColumn(
                name: "DestinationFacilityId",
                table: "RoutSteps");

            migrationBuilder.DropColumn(
                name: "GoodsDetailId",
                table: "RoutSteps");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "RoutSteps");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "RoutSteps");

            migrationBuilder.DropColumn(
                name: "OriginCityId",
                table: "RoutSteps");

            migrationBuilder.DropColumn(
                name: "PickingTypeId",
                table: "RoutSteps");

            migrationBuilder.DropColumn(
                name: "SourceFacilityId",
                table: "RoutSteps");

            migrationBuilder.DropColumn(
                name: "TrailerTypeId",
                table: "RoutSteps");

            migrationBuilder.DropColumn(
                name: "TrucksTypeId",
                table: "RoutSteps");

            migrationBuilder.DropColumn(
                name: "Adress",
                table: "Ports");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Ports");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Ports");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "PlateTypes");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "GoodsDetails");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "GoodsDetails");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "GoodsDetails");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "GoodCategories");

            migrationBuilder.DropColumn(
                name: "Adress",
                table: "Facilities");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Facilities");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Facilities");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Cities");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "TrucksTypes",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfTrips",
                table: "ShippingRequestVases",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "OtherVasName",
                table: "ShippingRequestVases",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "TrucksTypeId",
                table: "ShippingRequests",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "GoodCategoryId",
                table: "ShippingRequests",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<decimal>(
                name: "ActualCommissionValue",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ActualMinCommissionValue",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ActualPercentCommission",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<byte>(
                name: "BidStatus",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<string>(
                name: "CancelReason",
                table: "ShippingRequests",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CarrierPrice",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<byte>(
                name: "CarrierPriceType",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<decimal>(
                name: "CommissionValueSetting",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "DestinationCityId",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DraftStep",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndTripDate",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasAccident",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCarrierHaveInvoice",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDirectRequest",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDrafted",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPrePayed",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsShipperHaveInvoice",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "MinValueCommissionSetting",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfPacking",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfTrips",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OriginCityId",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherGoodsCategoryName",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherPackingTypeName",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherTransportTypeName",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherTrucksTypeName",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PackingTypeId",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PercentCommissionSetting",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ReferenceNumber",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "RequestType",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "RouteTypeId",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShippingTypeId",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartTripDate",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "Status",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotalAmount",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "TotalBids",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalCommission",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "TotalOffers",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "TotalWeight",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "TotalsTripsAddByShippier",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "VatAmount",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "VatSetting",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "price",
                table: "ShippingRequestBids",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<decimal>(
                name: "ActualCommissionValue",
                table: "ShippingRequestBids",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ActualMinCommissionValue",
                table: "ShippingRequestBids",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ActualPercentCommission",
                table: "ShippingRequestBids",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "BasePrice",
                table: "ShippingRequestBids",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceSubTotal",
                table: "ShippingRequestBids",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalCommission",
                table: "ShippingRequestBids",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "VatAmount",
                table: "ShippingRequestBids",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<long>(
                name: "ShippingRequestId",
                table: "RoutSteps",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "AssignedTruckId",
                table: "RoutSteps",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "AssignedDriverUserId",
                table: "RoutSteps",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "DestinationRoutPointId",
                table: "RoutSteps",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "ExistingAmount",
                table: "RoutSteps",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RemainingAmount",
                table: "RoutSteps",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "SourceRoutPointId",
                table: "RoutSteps",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "TotalAmount",
                table: "RoutSteps",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "OriginCityId",
                table: "Routes",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DestinationCityId",
                table: "Routes",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Ports",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Point>(
                name: "Location",
                table: "Ports",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BayanPlatetypeId",
                table: "PlateTypes",
                nullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Weight",
                table: "GoodsDetails",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "GoodCategoryId",
                table: "GoodsDetails",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Amount",
                table: "GoodsDetails",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DangerousGoodTypeId",
                table: "GoodsDetails",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherGoodsCategoryName",
                table: "GoodsDetails",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherUnitOfMeasureName",
                table: "GoodsDetails",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "RoutPointId",
                table: "GoodsDetails",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "FatherId",
                table: "GoodCategories",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "GoodCategories",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Facilities",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Point>(
                name: "Location",
                table: "Facilities",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DocumentRelatedWithId",
                table: "DocumentTypes",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TemplateContentType",
                table: "DocumentTypes",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TemplateId",
                table: "DocumentTypes",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TemplateName",
                table: "DocumentTypes",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "DocumentTypeId",
                table: "DocumentFiles",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<string>(
                name: "OtherDocumentTypeName",
                table: "DocumentFiles",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShippingRequestTripId",
                table: "DocumentFiles",
                nullable: true);

            migrationBuilder.AddColumn<Point>(
                name: "Location",
                table: "Cities",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountNumber",
                table: "AbpUsers",
                maxLength: 12,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountNumber",
                table: "AbpTenants",
                maxLength: 12,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Balance",
                table: "AbpTenants",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ContractNumber",
                table: "AbpTenants",
                maxLength: 12,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CreditBalance",
                table: "AbpTenants",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "MobileNo",
                table: "AbpTenants",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ReservedBalance",
                table: "AbpTenants",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "AppLocalizations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MasterKey = table.Column<string>(nullable: false),
                    MasterValue = table.Column<string>(nullable: true),
                    PlatForm = table.Column<byte>(nullable: false),
                    AppVersion = table.Column<int>(nullable: false),
                    Version = table.Column<byte>(nullable: false),
                    Section = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppLocalizations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BalanceRecharges",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    ReferenceNo = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BalanceRecharges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BalanceRecharges_AbpTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DangerousGoodTypes",
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
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    BayanIntegrationId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DangerousGoodTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GoodsCategoryTranslations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DisplayName = table.Column<string>(nullable: true),
                    CoreId = table.Column<int>(nullable: false),
                    Language = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoodsCategoryTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GoodsCategoryTranslations_GoodCategories_CoreId",
                        column: x => x.CoreId,
                        principalTable: "GoodCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InvoicePaymentMethods",
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
                    DisplayName = table.Column<string>(maxLength: 250, nullable: true),
                    PaymentType = table.Column<byte>(nullable: false),
                    InvoiceDueDateDays = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoicePaymentMethods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InvoicePeriods",
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
                    DisplayName = table.Column<string>(maxLength: 256, nullable: false),
                    Description = table.Column<string>(nullable: true),
                    PeriodType = table.Column<byte>(nullable: false),
                    FreqInterval = table.Column<int>(nullable: false),
                    FreqRelativeInterval = table.Column<byte>(nullable: false),
                    FreqRecurrence = table.Column<string>(nullable: true),
                    NextRunDate = table.Column<DateTime>(nullable: true),
                    ShipperOnlyUsed = table.Column<bool>(nullable: false),
                    Enabled = table.Column<bool>(nullable: false),
                    LastRunDate = table.Column<DateTime>(nullable: true),
                    Cronexpression = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoicePeriods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InvoicesProforma",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    RequestId = table.Column<long>(nullable: false),
                    TotalAmount = table.Column<decimal>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    VatAmount = table.Column<decimal>(nullable: false),
                    TaxVat = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoicesProforma", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoicesProforma_ShippingRequests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "ShippingRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvoicesProforma_AbpTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PackingTypes",
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
                    DisplayName = table.Column<string>(maxLength: 128, nullable: false),
                    Description = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackingTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlateTypeTranslations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CoreId = table.Column<int>(nullable: false),
                    Language = table.Column<string>(nullable: true),
                    DisplayName = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlateTypeTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlateTypeTranslations_PlateTypes_CoreId",
                        column: x => x.CoreId,
                        principalTable: "PlateTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PriceOffers",
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
                    ReferenceNumber = table.Column<long>(nullable: true),
                    ParentId = table.Column<long>(nullable: true),
                    ShippingRequestId = table.Column<long>(nullable: false),
                    SourceId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    Channel = table.Column<byte>(nullable: false),
                    Status = table.Column<byte>(nullable: false),
                    PriceType = table.Column<byte>(nullable: false),
                    ItemPrice = table.Column<decimal>(nullable: false),
                    ItemVatAmount = table.Column<decimal>(nullable: false),
                    ItemTotalAmount = table.Column<decimal>(nullable: false),
                    ItemSubTotalAmountWithCommission = table.Column<decimal>(nullable: false),
                    ItemVatAmountWithCommission = table.Column<decimal>(nullable: false),
                    ItemTotalAmountWithCommission = table.Column<decimal>(nullable: false),
                    TotalAmount = table.Column<decimal>(nullable: false),
                    SubTotalAmount = table.Column<decimal>(nullable: false),
                    VatAmount = table.Column<decimal>(nullable: false),
                    TotalAmountWithCommission = table.Column<decimal>(nullable: false),
                    SubTotalAmountWithCommission = table.Column<decimal>(nullable: false),
                    VatAmountWithCommission = table.Column<decimal>(nullable: false),
                    TaxVat = table.Column<decimal>(nullable: false),
                    CommissionType = table.Column<byte>(nullable: false),
                    ItemCommissionAmount = table.Column<decimal>(nullable: false),
                    CommissionPercentageOrAddValue = table.Column<decimal>(nullable: false),
                    CommissionAmount = table.Column<decimal>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    RejectedReason = table.Column<string>(nullable: true),
                    IsView = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceOffers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PriceOffers_PriceOffers_ParentId",
                        column: x => x.ParentId,
                        principalTable: "PriceOffers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PriceOffers_ShippingRequests_ShippingRequestId",
                        column: x => x.ShippingRequestId,
                        principalTable: "ShippingRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PriceOffers_AbpTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Receivers",
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
                    FullName = table.Column<string>(maxLength: 256, nullable: false),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    PhoneNumber = table.Column<string>(maxLength: 32, nullable: false),
                    FacilityId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Receivers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Receivers_Facilities_FacilityId",
                        column: x => x.FacilityId,
                        principalTable: "Facilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShippingRequestDirectRequests",
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
                    CarrierTenantId = table.Column<int>(nullable: false),
                    ShippingRequestId = table.Column<long>(nullable: false),
                    Status = table.Column<byte>(nullable: false),
                    RejetcReason = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingRequestDirectRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingRequestDirectRequests_AbpTenants_CarrierTenantId",
                        column: x => x.CarrierTenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShippingRequestDirectRequests_ShippingRequests_ShippingRequestId",
                        column: x => x.ShippingRequestId,
                        principalTable: "ShippingRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShippingRequestDirectRequests_AbpTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShippingRequestReasonAccidents",
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
                    DeletionTime = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingRequestReasonAccidents", x => x.Id);
                });

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
                    CarrirerTenantId = table.Column<int>(nullable: false),
                    RequestId = table.Column<long>(nullable: false),
                    Price = table.Column<decimal>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    RejetcReason = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingRequestsCarrierDirectPricing", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingRequestsCarrierDirectPricing_AbpTenants_CarrirerTenantId",
                        column: x => x.CarrirerTenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShippingRequestsCarrierDirectPricing_ShippingRequests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "ShippingRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShippingRequestsCarrierDirectPricing_AbpTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShippingRequestTripRejectReasons",
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
                    DeletionTime = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingRequestTripRejectReasons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShippingTypes",
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
                    DisplayName = table.Column<string>(maxLength: 128, nullable: false),
                    Description = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TenantCarriers",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(nullable: false),
                    CarrierTenantId = table.Column<int>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantCarriers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantCarriers_AbpTenants_CarrierTenantId",
                        column: x => x.CarrierTenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TenantCarriers_AbpTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChannelId = table.Column<byte>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    Count = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: true),
                    SourceId = table.Column<long>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_AbpTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserDeviceTokens",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(nullable: false),
                    DeviceId = table.Column<string>(nullable: true),
                    Token = table.Column<string>(nullable: true),
                    ExpireDate = table.Column<DateTime>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    LastModificationTime = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDeviceTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserDeviceTokens_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserOTPs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(nullable: false),
                    OTP = table.Column<string>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    ExpireTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserOTPs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserOTPs_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VasesTranslations",
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
                    DisplayName = table.Column<string>(maxLength: 256, nullable: true),
                    Language = table.Column<string>(nullable: true),
                    CoreId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VasesTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VasesTranslations_Vases_CoreId",
                        column: x => x.CoreId,
                        principalTable: "Vases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AppLocalizationTranslations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(nullable: false),
                    CoreId = table.Column<int>(nullable: false),
                    Language = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppLocalizationTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppLocalizationTranslations_AppLocalizations_CoreId",
                        column: x => x.CoreId,
                        principalTable: "AppLocalizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TerminologieEditions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EditionId = table.Column<int>(nullable: false),
                    TerminologieId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TerminologieEditions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TerminologieEditions_AbpEditions_EditionId",
                        column: x => x.EditionId,
                        principalTable: "AbpEditions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TerminologieEditions_AppLocalizations_TerminologieId",
                        column: x => x.TerminologieId,
                        principalTable: "AppLocalizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TerminologiePages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PageUrl = table.Column<string>(nullable: true),
                    TerminologieId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TerminologiePages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TerminologiePages_AppLocalizations_TerminologieId",
                        column: x => x.TerminologieId,
                        principalTable: "AppLocalizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GroupPeriods",
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
                    PeriodId = table.Column<int>(nullable: false),
                    Note = table.Column<string>(nullable: true),
                    AmountWithTaxVat = table.Column<decimal>(nullable: false),
                    VatAmount = table.Column<decimal>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    TaxVat = table.Column<decimal>(nullable: false),
                    DocumentId = table.Column<Guid>(nullable: true),
                    DocumentName = table.Column<string>(nullable: true),
                    DocumentContentType = table.Column<string>(nullable: true),
                    Status = table.Column<byte>(nullable: false),
                    RejectedReason = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupPeriods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupPeriods_InvoicePeriods_PeriodId",
                        column: x => x.PeriodId,
                        principalTable: "InvoicePeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GroupPeriods_AbpTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Invoices",
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
                    InvoiceNumber = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    PeriodId = table.Column<int>(nullable: false),
                    Channel = table.Column<byte>(nullable: false),
                    DueDate = table.Column<DateTime>(nullable: false),
                    IsPaid = table.Column<bool>(nullable: false),
                    Note = table.Column<string>(nullable: true),
                    SubTotalAmount = table.Column<decimal>(nullable: false),
                    TotalAmount = table.Column<decimal>(nullable: false),
                    VatAmount = table.Column<decimal>(nullable: false),
                    TaxVat = table.Column<decimal>(nullable: false),
                    AccountType = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invoices_InvoicePeriods_PeriodId",
                        column: x => x.PeriodId,
                        principalTable: "InvoicePeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Invoices_AbpTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubmitInvoices",
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
                    ReferencNumber = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    PeriodId = table.Column<int>(nullable: false),
                    Channel = table.Column<byte>(nullable: false),
                    SubTotalAmount = table.Column<decimal>(nullable: false),
                    TotalAmount = table.Column<decimal>(nullable: false),
                    VatAmount = table.Column<decimal>(nullable: false),
                    TaxVat = table.Column<decimal>(nullable: false),
                    DocumentId = table.Column<Guid>(nullable: true),
                    DocumentName = table.Column<string>(nullable: true),
                    DocumentContentType = table.Column<string>(nullable: true),
                    Status = table.Column<byte>(nullable: false),
                    RejectedReason = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubmitInvoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubmitInvoices_InvoicePeriods_PeriodId",
                        column: x => x.PeriodId,
                        principalTable: "InvoicePeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubmitInvoices_AbpTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PackingTypesTranslations",
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
                    DisplayName = table.Column<string>(maxLength: 128, nullable: false),
                    Description = table.Column<string>(maxLength: 256, nullable: true),
                    Language = table.Column<string>(nullable: false),
                    CoreId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackingTypesTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PackingTypesTranslations_PackingTypes_CoreId",
                        column: x => x.CoreId,
                        principalTable: "PackingTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PriceOfferDetails",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PriceOfferId = table.Column<long>(nullable: false),
                    SourceId = table.Column<long>(nullable: true),
                    PriceType = table.Column<byte>(nullable: false),
                    ItemPrice = table.Column<decimal>(nullable: false),
                    ItemVatAmount = table.Column<decimal>(nullable: false),
                    ItemTotalAmount = table.Column<decimal>(nullable: false),
                    ItemSubTotalAmountWithCommission = table.Column<decimal>(nullable: false),
                    ItemVatAmountWithCommission = table.Column<decimal>(nullable: false),
                    ItemTotalAmountWithCommission = table.Column<decimal>(nullable: false),
                    TotalAmount = table.Column<decimal>(nullable: false),
                    SubTotalAmount = table.Column<decimal>(nullable: false),
                    VatAmount = table.Column<decimal>(nullable: false),
                    TotalAmountWithCommission = table.Column<decimal>(nullable: false),
                    SubTotalAmountWithCommission = table.Column<decimal>(nullable: false),
                    VatAmountWithCommission = table.Column<decimal>(nullable: false),
                    ItemCommissionAmount = table.Column<decimal>(nullable: false),
                    CommissionAmount = table.Column<decimal>(nullable: false),
                    CommissionType = table.Column<byte>(nullable: false),
                    CommissionPercentageOrAddValue = table.Column<decimal>(nullable: false),
                    Quantity = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceOfferDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PriceOfferDetails_PriceOffers_PriceOfferId",
                        column: x => x.PriceOfferId,
                        principalTable: "PriceOffers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShippingRequestReasonAccidentTranslations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 60, nullable: false),
                    CoreId = table.Column<int>(nullable: false),
                    Language = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingRequestReasonAccidentTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingRequestReasonAccidentTranslations_ShippingRequestReasonAccidents_CoreId",
                        column: x => x.CoreId,
                        principalTable: "ShippingRequestReasonAccidents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TachyonPriceOffers",
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
                    CarrirerTenantId = table.Column<int>(nullable: true),
                    ShippingRequestId = table.Column<long>(nullable: false),
                    ShippingRequestBidId = table.Column<long>(nullable: true),
                    ShippingRequestCarrierDirectPricingId = table.Column<int>(nullable: true),
                    OfferStatus = table.Column<byte>(nullable: false),
                    RejectedReason = table.Column<string>(nullable: true),
                    PriceType = table.Column<byte>(nullable: false),
                    CarrierPrice = table.Column<decimal>(nullable: true),
                    SubTotalAmount = table.Column<decimal>(nullable: true),
                    TotalAmount = table.Column<decimal>(nullable: false),
                    VatAmount = table.Column<decimal>(nullable: true),
                    TotalCommission = table.Column<decimal>(nullable: true),
                    VatSetting = table.Column<decimal>(nullable: true),
                    CommissionValueSetting = table.Column<decimal>(nullable: true),
                    PercentCommissionSetting = table.Column<decimal>(nullable: true),
                    MinCommissionValueSetting = table.Column<decimal>(nullable: true),
                    ActualCommissionValue = table.Column<decimal>(nullable: true),
                    ActualPercentCommission = table.Column<decimal>(nullable: true),
                    ActualMinCommissionValue = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TachyonPriceOffers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TachyonPriceOffers_AbpTenants_CarrirerTenantId",
                        column: x => x.CarrirerTenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TachyonPriceOffers_ShippingRequestBids_ShippingRequestBidId",
                        column: x => x.ShippingRequestBidId,
                        principalTable: "ShippingRequestBids",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TachyonPriceOffers_ShippingRequestsCarrierDirectPricing_ShippingRequestCarrierDirectPricingId",
                        column: x => x.ShippingRequestCarrierDirectPricingId,
                        principalTable: "ShippingRequestsCarrierDirectPricing",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TachyonPriceOffers_ShippingRequests_ShippingRequestId",
                        column: x => x.ShippingRequestId,
                        principalTable: "ShippingRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShippingRequestTripRejectReasonTranslations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 60, nullable: false),
                    CoreId = table.Column<int>(nullable: false),
                    Language = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingRequestTripRejectReasonTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingRequestTripRejectReasonTranslations_ShippingRequestTripRejectReasons_CoreId",
                        column: x => x.CoreId,
                        principalTable: "ShippingRequestTripRejectReasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShippingRequestTrips",
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
                    WaybillNumber = table.Column<long>(nullable: true),
                    StartTripDate = table.Column<DateTime>(nullable: false),
                    EndTripDate = table.Column<DateTime>(nullable: true),
                    StartWorking = table.Column<DateTime>(nullable: true),
                    EndWorking = table.Column<DateTime>(nullable: true),
                    HasAttachment = table.Column<bool>(nullable: false),
                    NeedsDeliveryNote = table.Column<bool>(nullable: false),
                    Status = table.Column<byte>(nullable: false),
                    RoutePointStatus = table.Column<byte>(nullable: false),
                    AssignedDriverUserId = table.Column<long>(nullable: true),
                    AssignedDriverTime = table.Column<DateTime>(nullable: true),
                    HasAccident = table.Column<bool>(nullable: false),
                    IsApproveCancledByShipper = table.Column<bool>(nullable: false),
                    IsApproveCancledByCarrier = table.Column<bool>(nullable: false),
                    AssignedTruckId = table.Column<long>(nullable: true),
                    ShippingRequestId = table.Column<long>(nullable: false),
                    OriginFacilityId = table.Column<long>(nullable: true),
                    DestinationFacilityId = table.Column<long>(nullable: true),
                    DriverStatus = table.Column<int>(nullable: false),
                    RejectReasonId = table.Column<int>(nullable: true),
                    RejectedReason = table.Column<string>(nullable: true),
                    TotalValue = table.Column<string>(nullable: true),
                    Note = table.Column<string>(maxLength: 512, nullable: true),
                    IsShipperHaveInvoice = table.Column<bool>(nullable: false),
                    IsCarrierHaveInvoice = table.Column<bool>(nullable: false),
                    TotalAmount = table.Column<decimal>(nullable: true),
                    SubTotalAmount = table.Column<decimal>(nullable: true),
                    VatAmount = table.Column<decimal>(nullable: true),
                    TotalAmountWithCommission = table.Column<decimal>(nullable: true),
                    SubTotalAmountWithCommission = table.Column<decimal>(nullable: true),
                    VatAmountWithCommission = table.Column<decimal>(nullable: true),
                    TaxVat = table.Column<decimal>(nullable: true),
                    CommissionType = table.Column<byte>(nullable: true),
                    CommissionPercentageOrAddValue = table.Column<decimal>(nullable: true),
                    CommissionAmount = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingRequestTrips", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingRequestTrips_AbpUsers_AssignedDriverUserId",
                        column: x => x.AssignedDriverUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShippingRequestTrips_Trucks_AssignedTruckId",
                        column: x => x.AssignedTruckId,
                        principalTable: "Trucks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShippingRequestTrips_Facilities_DestinationFacilityId",
                        column: x => x.DestinationFacilityId,
                        principalTable: "Facilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShippingRequestTrips_Facilities_OriginFacilityId",
                        column: x => x.OriginFacilityId,
                        principalTable: "Facilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShippingRequestTrips_ShippingRequestTripRejectReasons_RejectReasonId",
                        column: x => x.RejectReasonId,
                        principalTable: "ShippingRequestTripRejectReasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShippingRequestTrips_ShippingRequests_ShippingRequestId",
                        column: x => x.ShippingRequestId,
                        principalTable: "ShippingRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShippingTypeTranslations",
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
                    DisplayName = table.Column<string>(maxLength: 128, nullable: false),
                    Description = table.Column<string>(maxLength: 256, nullable: true),
                    Language = table.Column<string>(nullable: true),
                    CoreId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingTypeTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingTypeTranslations_ShippingTypes_CoreId",
                        column: x => x.CoreId,
                        principalTable: "ShippingTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GroupShippingRequests",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestId = table.Column<long>(nullable: false),
                    GroupId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupShippingRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupShippingRequests_GroupPeriods_GroupId",
                        column: x => x.GroupId,
                        principalTable: "GroupPeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GroupShippingRequests_ShippingRequests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "ShippingRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GroupPeriodsInvoices",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceId = table.Column<long>(nullable: false),
                    GroupId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupPeriodsInvoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupPeriodsInvoices_GroupPeriods_GroupId",
                        column: x => x.GroupId,
                        principalTable: "GroupPeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GroupPeriodsInvoices_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceTrips",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceId = table.Column<long>(nullable: false),
                    TripId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceTrips", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoiceTrips_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvoiceTrips_ShippingRequestTrips_TripId",
                        column: x => x.TripId,
                        principalTable: "ShippingRequestTrips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoutPoints",
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
                    WaybillNumber = table.Column<long>(nullable: true),
                    DisplayName = table.Column<string>(nullable: true),
                    PickingType = table.Column<byte>(nullable: false),
                    FacilityId = table.Column<long>(nullable: false),
                    ShippingRequestTripId = table.Column<int>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    Status = table.Column<byte>(nullable: false),
                    StartTime = table.Column<DateTime>(nullable: true),
                    EndTime = table.Column<DateTime>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    IsComplete = table.Column<bool>(nullable: false),
                    DocumentId = table.Column<Guid>(nullable: true),
                    DocumentName = table.Column<string>(nullable: true),
                    DocumentContentType = table.Column<string>(nullable: true),
                    Rating = table.Column<double>(nullable: true),
                    ReceiverNote = table.Column<string>(nullable: true),
                    ReceiverId = table.Column<int>(nullable: true),
                    ReceiverFullName = table.Column<string>(nullable: true),
                    ReceiverPhoneNumber = table.Column<string>(nullable: true),
                    ReceiverCardIdNumber = table.Column<string>(nullable: true),
                    ReceiverEmailAddress = table.Column<string>(nullable: true),
                    Note = table.Column<string>(nullable: true),
                    IsDeliveryNoteUploaded = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoutPoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoutPoints_Facilities_FacilityId",
                        column: x => x.FacilityId,
                        principalTable: "Facilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoutPoints_Receivers_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "Receivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoutPoints_ShippingRequestTrips_ShippingRequestTripId",
                        column: x => x.ShippingRequestTripId,
                        principalTable: "ShippingRequestTrips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShippingRequestTripVases",
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
                    ShippingRequestVasId = table.Column<long>(nullable: false),
                    ShippingRequestTripId = table.Column<int>(nullable: false),
                    TotalAmount = table.Column<decimal>(nullable: true),
                    SubTotalAmount = table.Column<decimal>(nullable: true),
                    VatAmount = table.Column<decimal>(nullable: true),
                    TotalAmountWithCommission = table.Column<decimal>(nullable: true),
                    SubTotalAmountWithCommission = table.Column<decimal>(nullable: true),
                    VatAmountWithCommission = table.Column<decimal>(nullable: true),
                    CommissionType = table.Column<byte>(nullable: true),
                    CommissionAmount = table.Column<decimal>(nullable: true),
                    CommissionPercentageOrAddValue = table.Column<decimal>(nullable: true),
                    Quantity = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingRequestTripVases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingRequestTripVases_ShippingRequestTrips_ShippingRequestTripId",
                        column: x => x.ShippingRequestTripId,
                        principalTable: "ShippingRequestTrips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShippingRequestTripVases_ShippingRequestVases_ShippingRequestVasId",
                        column: x => x.ShippingRequestVasId,
                        principalTable: "ShippingRequestVases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubmitInvoiceTrips",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubmitId = table.Column<long>(nullable: false),
                    TripId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubmitInvoiceTrips", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubmitInvoiceTrips_SubmitInvoices_SubmitId",
                        column: x => x.SubmitId,
                        principalTable: "SubmitInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubmitInvoiceTrips_ShippingRequestTrips_TripId",
                        column: x => x.TripId,
                        principalTable: "ShippingRequestTrips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoutPointDocuments",
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
                    RoutPointId = table.Column<long>(nullable: false),
                    RoutePointDocumentType = table.Column<byte>(nullable: false),
                    DocumentId = table.Column<Guid>(nullable: true),
                    DocumentName = table.Column<string>(nullable: true),
                    DocumentContentType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoutPointDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoutPointDocuments_RoutPoints_RoutPointId",
                        column: x => x.RoutPointId,
                        principalTable: "RoutPoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoutPointStatusTransitions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PointId = table.Column<long>(nullable: false),
                    Status = table.Column<byte>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoutPointStatusTransitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoutPointStatusTransitions_RoutPoints_PointId",
                        column: x => x.PointId,
                        principalTable: "RoutPoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShippingRequestTripAccidents",
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
                    PointId = table.Column<long>(nullable: false),
                    ReasoneId = table.Column<int>(nullable: true),
                    OtherReasonName = table.Column<string>(nullable: true),
                    Description = table.Column<string>(maxLength: 500, nullable: true),
                    IsResolve = table.Column<bool>(nullable: false),
                    DocumentId = table.Column<Guid>(nullable: true),
                    DocumentName = table.Column<string>(nullable: true),
                    DocumentContentType = table.Column<string>(nullable: true),
                    Location = table.Column<Point>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingRequestTripAccidents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingRequestTripAccidents_RoutPoints_PointId",
                        column: x => x.PointId,
                        principalTable: "RoutPoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShippingRequestTripAccidents_ShippingRequestReasonAccidents_ReasoneId",
                        column: x => x.ReasoneId,
                        principalTable: "ShippingRequestReasonAccidents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShippingRequestTripTransitions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FromPointId = table.Column<long>(nullable: true),
                    FromLocation = table.Column<Point>(nullable: true),
                    ToPointId = table.Column<long>(nullable: false),
                    ToLocation = table.Column<Point>(nullable: true),
                    IsComplete = table.Column<bool>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingRequestTripTransitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingRequestTripTransitions_RoutPoints_FromPointId",
                        column: x => x.FromPointId,
                        principalTable: "RoutPoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShippingRequestTripTransitions_RoutPoints_ToPointId",
                        column: x => x.ToPointId,
                        principalTable: "RoutPoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShippingRequestTripAccidentResolves",
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
                    AccidentId = table.Column<int>(nullable: false),
                    Description = table.Column<string>(maxLength: 500, nullable: true),
                    DocumentId = table.Column<Guid>(nullable: true),
                    DocumentName = table.Column<string>(nullable: true),
                    DocumentContentType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingRequestTripAccidentResolves", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingRequestTripAccidentResolves_ShippingRequestTripAccidents_AccidentId",
                        column: x => x.AccidentId,
                        principalTable: "ShippingRequestTripAccidents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequests_DestinationCityId",
                table: "ShippingRequests",
                column: "DestinationCityId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequests_OriginCityId",
                table: "ShippingRequests",
                column: "OriginCityId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequests_PackingTypeId",
                table: "ShippingRequests",
                column: "PackingTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequests_ReferenceNumber",
                table: "ShippingRequests",
                column: "ReferenceNumber",
                unique: true,
                filter: "[ReferenceNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequests_ShippingTypeId",
                table: "ShippingRequests",
                column: "ShippingTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutSteps_DestinationRoutPointId",
                table: "RoutSteps",
                column: "DestinationRoutPointId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutSteps_SourceRoutPointId",
                table: "RoutSteps",
                column: "SourceRoutPointId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsDetails_DangerousGoodTypeId",
                table: "GoodsDetails",
                column: "DangerousGoodTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsDetails_RoutPointId",
                table: "GoodsDetails",
                column: "RoutPointId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodCategories_FatherId",
                table: "GoodCategories",
                column: "FatherId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTypes_DocumentRelatedWithId",
                table: "DocumentTypes",
                column: "DocumentRelatedWithId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentFiles_ShippingRequestTripId",
                table: "DocumentFiles",
                column: "ShippingRequestTripId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpUsers_AccountNumber",
                table: "AbpUsers",
                column: "AccountNumber",
                unique: true,
                filter: "[AccountNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_AccountNumber",
                table: "AbpTenants",
                column: "AccountNumber",
                unique: true,
                filter: "[AccountNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_ContractNumber",
                table: "AbpTenants",
                column: "ContractNumber",
                unique: true,
                filter: "[ContractNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AppLocalizationTranslations_CoreId",
                table: "AppLocalizationTranslations",
                column: "CoreId");

            migrationBuilder.CreateIndex(
                name: "IX_BalanceRecharges_TenantId",
                table: "BalanceRecharges",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsCategoryTranslations_CoreId",
                table: "GoodsCategoryTranslations",
                column: "CoreId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupPeriods_PeriodId",
                table: "GroupPeriods",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupPeriods_TenantId",
                table: "GroupPeriods",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupPeriodsInvoices_GroupId",
                table: "GroupPeriodsInvoices",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupPeriodsInvoices_InvoiceId",
                table: "GroupPeriodsInvoices",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupShippingRequests_GroupId",
                table: "GroupShippingRequests",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupShippingRequests_RequestId",
                table: "GroupShippingRequests",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_InvoiceNumber",
                table: "Invoices",
                column: "InvoiceNumber",
                unique: true,
                filter: "[InvoiceNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_PeriodId",
                table: "Invoices",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_TenantId",
                table: "Invoices",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoicesProforma_RequestId",
                table: "InvoicesProforma",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoicesProforma_TenantId",
                table: "InvoicesProforma",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceTrips_InvoiceId",
                table: "InvoiceTrips",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceTrips_TripId",
                table: "InvoiceTrips",
                column: "TripId");

            migrationBuilder.CreateIndex(
                name: "IX_PackingTypesTranslations_CoreId",
                table: "PackingTypesTranslations",
                column: "CoreId");

            migrationBuilder.CreateIndex(
                name: "IX_PlateTypeTranslations_CoreId",
                table: "PlateTypeTranslations",
                column: "CoreId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceOfferDetails_PriceOfferId",
                table: "PriceOfferDetails",
                column: "PriceOfferId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceOffers_ParentId",
                table: "PriceOffers",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceOffers_ShippingRequestId",
                table: "PriceOffers",
                column: "ShippingRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceOffers_TenantId",
                table: "PriceOffers",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Receivers_FacilityId",
                table: "Receivers",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_Receivers_TenantId",
                table: "Receivers",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutPointDocuments_RoutPointId",
                table: "RoutPointDocuments",
                column: "RoutPointId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutPoints_FacilityId",
                table: "RoutPoints",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutPoints_ReceiverId",
                table: "RoutPoints",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutPoints_ShippingRequestTripId",
                table: "RoutPoints",
                column: "ShippingRequestTripId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutPoints_WaybillNumber",
                table: "RoutPoints",
                column: "WaybillNumber",
                unique: true,
                filter: "[WaybillNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_RoutPointStatusTransitions_PointId",
                table: "RoutPointStatusTransitions",
                column: "PointId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestDirectRequests_CarrierTenantId",
                table: "ShippingRequestDirectRequests",
                column: "CarrierTenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestDirectRequests_ShippingRequestId",
                table: "ShippingRequestDirectRequests",
                column: "ShippingRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestDirectRequests_TenantId",
                table: "ShippingRequestDirectRequests",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestReasonAccidentTranslations_CoreId",
                table: "ShippingRequestReasonAccidentTranslations",
                column: "CoreId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestsCarrierDirectPricing_CarrirerTenantId",
                table: "ShippingRequestsCarrierDirectPricing",
                column: "CarrirerTenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestsCarrierDirectPricing_RequestId",
                table: "ShippingRequestsCarrierDirectPricing",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestsCarrierDirectPricing_TenantId",
                table: "ShippingRequestsCarrierDirectPricing",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTripAccidentResolves_AccidentId",
                table: "ShippingRequestTripAccidentResolves",
                column: "AccidentId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTripAccidents_PointId",
                table: "ShippingRequestTripAccidents",
                column: "PointId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTripAccidents_ReasoneId",
                table: "ShippingRequestTripAccidents",
                column: "ReasoneId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTripRejectReasonTranslations_CoreId",
                table: "ShippingRequestTripRejectReasonTranslations",
                column: "CoreId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTrips_AssignedDriverUserId",
                table: "ShippingRequestTrips",
                column: "AssignedDriverUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTrips_AssignedTruckId",
                table: "ShippingRequestTrips",
                column: "AssignedTruckId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTrips_DestinationFacilityId",
                table: "ShippingRequestTrips",
                column: "DestinationFacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTrips_OriginFacilityId",
                table: "ShippingRequestTrips",
                column: "OriginFacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTrips_RejectReasonId",
                table: "ShippingRequestTrips",
                column: "RejectReasonId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTrips_ShippingRequestId",
                table: "ShippingRequestTrips",
                column: "ShippingRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTrips_WaybillNumber",
                table: "ShippingRequestTrips",
                column: "WaybillNumber",
                unique: true,
                filter: "[WaybillNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTripTransitions_FromPointId",
                table: "ShippingRequestTripTransitions",
                column: "FromPointId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTripTransitions_ToPointId",
                table: "ShippingRequestTripTransitions",
                column: "ToPointId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTripVases_ShippingRequestTripId",
                table: "ShippingRequestTripVases",
                column: "ShippingRequestTripId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTripVases_ShippingRequestVasId",
                table: "ShippingRequestTripVases",
                column: "ShippingRequestVasId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingTypeTranslations_CoreId",
                table: "ShippingTypeTranslations",
                column: "CoreId");

            migrationBuilder.CreateIndex(
                name: "IX_SubmitInvoices_PeriodId",
                table: "SubmitInvoices",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_SubmitInvoices_TenantId",
                table: "SubmitInvoices",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SubmitInvoiceTrips_SubmitId",
                table: "SubmitInvoiceTrips",
                column: "SubmitId");

            migrationBuilder.CreateIndex(
                name: "IX_SubmitInvoiceTrips_TripId",
                table: "SubmitInvoiceTrips",
                column: "TripId");

            migrationBuilder.CreateIndex(
                name: "IX_TachyonPriceOffers_CarrirerTenantId",
                table: "TachyonPriceOffers",
                column: "CarrirerTenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TachyonPriceOffers_ShippingRequestBidId",
                table: "TachyonPriceOffers",
                column: "ShippingRequestBidId");

            migrationBuilder.CreateIndex(
                name: "IX_TachyonPriceOffers_ShippingRequestCarrierDirectPricingId",
                table: "TachyonPriceOffers",
                column: "ShippingRequestCarrierDirectPricingId");

            migrationBuilder.CreateIndex(
                name: "IX_TachyonPriceOffers_ShippingRequestId",
                table: "TachyonPriceOffers",
                column: "ShippingRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_TachyonPriceOffers_TenantId",
                table: "TachyonPriceOffers",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantCarriers_CarrierTenantId",
                table: "TenantCarriers",
                column: "CarrierTenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantCarriers_TenantId",
                table: "TenantCarriers",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TerminologieEditions_EditionId",
                table: "TerminologieEditions",
                column: "EditionId");

            migrationBuilder.CreateIndex(
                name: "IX_TerminologieEditions_TerminologieId",
                table: "TerminologieEditions",
                column: "TerminologieId");

            migrationBuilder.CreateIndex(
                name: "IX_TerminologiePages_TerminologieId",
                table: "TerminologiePages",
                column: "TerminologieId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_TenantId",
                table: "Transactions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDeviceTokens_UserId",
                table: "UserDeviceTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserOTPs_UserId",
                table: "UserOTPs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_VasesTranslations_CoreId",
                table: "VasesTranslations",
                column: "CoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentFiles_DocumentTypes_DocumentTypeId",
                table: "DocumentFiles",
                column: "DocumentTypeId",
                principalTable: "DocumentTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentFiles_ShippingRequestTrips_ShippingRequestTripId",
                table: "DocumentFiles",
                column: "ShippingRequestTripId",
                principalTable: "ShippingRequestTrips",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentTypes_AbpTenants_DocumentRelatedWithId",
                table: "DocumentTypes",
                column: "DocumentRelatedWithId",
                principalTable: "AbpTenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GoodCategories_GoodCategories_FatherId",
                table: "GoodCategories",
                column: "FatherId",
                principalTable: "GoodCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GoodsDetails_DangerousGoodTypes_DangerousGoodTypeId",
                table: "GoodsDetails",
                column: "DangerousGoodTypeId",
                principalTable: "DangerousGoodTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GoodsDetails_GoodCategories_GoodCategoryId",
                table: "GoodsDetails",
                column: "GoodCategoryId",
                principalTable: "GoodCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GoodsDetails_RoutPoints_RoutPointId",
                table: "GoodsDetails",
                column: "RoutPointId",
                principalTable: "RoutPoints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Routes_Cities_DestinationCityId",
                table: "Routes",
                column: "DestinationCityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Routes_Cities_OriginCityId",
                table: "Routes",
                column: "OriginCityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoutSteps_AbpUsers_AssignedDriverUserId",
                table: "RoutSteps",
                column: "AssignedDriverUserId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoutSteps_Trucks_AssignedTruckId",
                table: "RoutSteps",
                column: "AssignedTruckId",
                principalTable: "Trucks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoutSteps_RoutPoints_DestinationRoutPointId",
                table: "RoutSteps",
                column: "DestinationRoutPointId",
                principalTable: "RoutPoints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoutSteps_ShippingRequests_ShippingRequestId",
                table: "RoutSteps",
                column: "ShippingRequestId",
                principalTable: "ShippingRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoutSteps_RoutPoints_SourceRoutPointId",
                table: "RoutSteps",
                column: "SourceRoutPointId",
                principalTable: "RoutPoints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequests_Cities_DestinationCityId",
                table: "ShippingRequests",
                column: "DestinationCityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequests_GoodCategories_GoodCategoryId",
                table: "ShippingRequests",
                column: "GoodCategoryId",
                principalTable: "GoodCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequests_Cities_OriginCityId",
                table: "ShippingRequests",
                column: "OriginCityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequests_PackingTypes_PackingTypeId",
                table: "ShippingRequests",
                column: "PackingTypeId",
                principalTable: "PackingTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequests_ShippingTypes_ShippingTypeId",
                table: "ShippingRequests",
                column: "ShippingTypeId",
                principalTable: "ShippingTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequests_TrucksTypes_TrucksTypeId",
                table: "ShippingRequests",
                column: "TrucksTypeId",
                principalTable: "TrucksTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentFiles_DocumentTypes_DocumentTypeId",
                table: "DocumentFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_DocumentFiles_ShippingRequestTrips_ShippingRequestTripId",
                table: "DocumentFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_DocumentTypes_AbpTenants_DocumentRelatedWithId",
                table: "DocumentTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_GoodCategories_GoodCategories_FatherId",
                table: "GoodCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_GoodsDetails_DangerousGoodTypes_DangerousGoodTypeId",
                table: "GoodsDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_GoodsDetails_GoodCategories_GoodCategoryId",
                table: "GoodsDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_GoodsDetails_RoutPoints_RoutPointId",
                table: "GoodsDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Routes_Cities_DestinationCityId",
                table: "Routes");

            migrationBuilder.DropForeignKey(
                name: "FK_Routes_Cities_OriginCityId",
                table: "Routes");

            migrationBuilder.DropForeignKey(
                name: "FK_RoutSteps_AbpUsers_AssignedDriverUserId",
                table: "RoutSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_RoutSteps_Trucks_AssignedTruckId",
                table: "RoutSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_RoutSteps_RoutPoints_DestinationRoutPointId",
                table: "RoutSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_RoutSteps_ShippingRequests_ShippingRequestId",
                table: "RoutSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_RoutSteps_RoutPoints_SourceRoutPointId",
                table: "RoutSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_Cities_DestinationCityId",
                table: "ShippingRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_GoodCategories_GoodCategoryId",
                table: "ShippingRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_Cities_OriginCityId",
                table: "ShippingRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_PackingTypes_PackingTypeId",
                table: "ShippingRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_ShippingTypes_ShippingTypeId",
                table: "ShippingRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_TrucksTypes_TrucksTypeId",
                table: "ShippingRequests");

            migrationBuilder.DropTable(
                name: "AppLocalizationTranslations");

            migrationBuilder.DropTable(
                name: "BalanceRecharges");

            migrationBuilder.DropTable(
                name: "DangerousGoodTypes");

            migrationBuilder.DropTable(
                name: "GoodsCategoryTranslations");

            migrationBuilder.DropTable(
                name: "GroupPeriodsInvoices");

            migrationBuilder.DropTable(
                name: "GroupShippingRequests");

            migrationBuilder.DropTable(
                name: "InvoicePaymentMethods");

            migrationBuilder.DropTable(
                name: "InvoicesProforma");

            migrationBuilder.DropTable(
                name: "InvoiceTrips");

            migrationBuilder.DropTable(
                name: "PackingTypesTranslations");

            migrationBuilder.DropTable(
                name: "PlateTypeTranslations");

            migrationBuilder.DropTable(
                name: "PriceOfferDetails");

            migrationBuilder.DropTable(
                name: "RoutPointDocuments");

            migrationBuilder.DropTable(
                name: "RoutPointStatusTransitions");

            migrationBuilder.DropTable(
                name: "ShippingRequestDirectRequests");

            migrationBuilder.DropTable(
                name: "ShippingRequestReasonAccidentTranslations");

            migrationBuilder.DropTable(
                name: "ShippingRequestTripAccidentResolves");

            migrationBuilder.DropTable(
                name: "ShippingRequestTripRejectReasonTranslations");

            migrationBuilder.DropTable(
                name: "ShippingRequestTripTransitions");

            migrationBuilder.DropTable(
                name: "ShippingRequestTripVases");

            migrationBuilder.DropTable(
                name: "ShippingTypeTranslations");

            migrationBuilder.DropTable(
                name: "SubmitInvoiceTrips");

            migrationBuilder.DropTable(
                name: "TachyonPriceOffers");

            migrationBuilder.DropTable(
                name: "TenantCarriers");

            migrationBuilder.DropTable(
                name: "TerminologieEditions");

            migrationBuilder.DropTable(
                name: "TerminologiePages");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "UserDeviceTokens");

            migrationBuilder.DropTable(
                name: "UserOTPs");

            migrationBuilder.DropTable(
                name: "VasesTranslations");

            migrationBuilder.DropTable(
                name: "GroupPeriods");

            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropTable(
                name: "PackingTypes");

            migrationBuilder.DropTable(
                name: "PriceOffers");

            migrationBuilder.DropTable(
                name: "ShippingRequestTripAccidents");

            migrationBuilder.DropTable(
                name: "ShippingTypes");

            migrationBuilder.DropTable(
                name: "SubmitInvoices");

            migrationBuilder.DropTable(
                name: "ShippingRequestsCarrierDirectPricing");

            migrationBuilder.DropTable(
                name: "AppLocalizations");

            migrationBuilder.DropTable(
                name: "RoutPoints");

            migrationBuilder.DropTable(
                name: "ShippingRequestReasonAccidents");

            migrationBuilder.DropTable(
                name: "InvoicePeriods");

            migrationBuilder.DropTable(
                name: "Receivers");

            migrationBuilder.DropTable(
                name: "ShippingRequestTrips");

            migrationBuilder.DropTable(
                name: "ShippingRequestTripRejectReasons");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequests_DestinationCityId",
                table: "ShippingRequests");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequests_OriginCityId",
                table: "ShippingRequests");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequests_PackingTypeId",
                table: "ShippingRequests");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequests_ReferenceNumber",
                table: "ShippingRequests");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequests_ShippingTypeId",
                table: "ShippingRequests");

            migrationBuilder.DropIndex(
                name: "IX_RoutSteps_DestinationRoutPointId",
                table: "RoutSteps");

            migrationBuilder.DropIndex(
                name: "IX_RoutSteps_SourceRoutPointId",
                table: "RoutSteps");

            migrationBuilder.DropIndex(
                name: "IX_GoodsDetails_DangerousGoodTypeId",
                table: "GoodsDetails");

            migrationBuilder.DropIndex(
                name: "IX_GoodsDetails_RoutPointId",
                table: "GoodsDetails");

            migrationBuilder.DropIndex(
                name: "IX_GoodCategories_FatherId",
                table: "GoodCategories");

            migrationBuilder.DropIndex(
                name: "IX_DocumentTypes_DocumentRelatedWithId",
                table: "DocumentTypes");

            migrationBuilder.DropIndex(
                name: "IX_DocumentFiles_ShippingRequestTripId",
                table: "DocumentFiles");

            migrationBuilder.DropIndex(
                name: "IX_AbpUsers_AccountNumber",
                table: "AbpUsers");

            migrationBuilder.DropIndex(
                name: "IX_AbpTenants_AccountNumber",
                table: "AbpTenants");

            migrationBuilder.DropIndex(
                name: "IX_AbpTenants_ContractNumber",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "TrucksTypes");

            migrationBuilder.DropColumn(
                name: "NumberOfTrips",
                table: "ShippingRequestVases");

            migrationBuilder.DropColumn(
                name: "OtherVasName",
                table: "ShippingRequestVases");

            migrationBuilder.DropColumn(
                name: "ActualCommissionValue",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "ActualMinCommissionValue",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "ActualPercentCommission",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "BidStatus",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "CancelReason",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "CarrierPrice",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "CarrierPriceType",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "CommissionValueSetting",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "DestinationCityId",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "DraftStep",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "EndTripDate",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "HasAccident",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "IsCarrierHaveInvoice",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "IsDirectRequest",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "IsDrafted",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "IsPrePayed",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "IsShipperHaveInvoice",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "MinValueCommissionSetting",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "NumberOfPacking",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "NumberOfTrips",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "OriginCityId",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "OtherGoodsCategoryName",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "OtherPackingTypeName",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "OtherTransportTypeName",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "OtherTrucksTypeName",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "PackingTypeId",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "PercentCommissionSetting",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "ReferenceNumber",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "RequestType",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "RouteTypeId",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "ShippingTypeId",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "StartTripDate",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "SubTotalAmount",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "TotalBids",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "TotalCommission",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "TotalOffers",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "TotalWeight",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "TotalsTripsAddByShippier",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "VatAmount",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "VatSetting",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "ActualCommissionValue",
                table: "ShippingRequestBids");

            migrationBuilder.DropColumn(
                name: "ActualMinCommissionValue",
                table: "ShippingRequestBids");

            migrationBuilder.DropColumn(
                name: "ActualPercentCommission",
                table: "ShippingRequestBids");

            migrationBuilder.DropColumn(
                name: "BasePrice",
                table: "ShippingRequestBids");

            migrationBuilder.DropColumn(
                name: "PriceSubTotal",
                table: "ShippingRequestBids");

            migrationBuilder.DropColumn(
                name: "TotalCommission",
                table: "ShippingRequestBids");

            migrationBuilder.DropColumn(
                name: "VatAmount",
                table: "ShippingRequestBids");

            migrationBuilder.DropColumn(
                name: "DestinationRoutPointId",
                table: "RoutSteps");

            migrationBuilder.DropColumn(
                name: "ExistingAmount",
                table: "RoutSteps");

            migrationBuilder.DropColumn(
                name: "RemainingAmount",
                table: "RoutSteps");

            migrationBuilder.DropColumn(
                name: "SourceRoutPointId",
                table: "RoutSteps");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "RoutSteps");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Ports");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Ports");

            migrationBuilder.DropColumn(
                name: "BayanPlatetypeId",
                table: "PlateTypes");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "GoodsDetails");

            migrationBuilder.DropColumn(
                name: "DangerousGoodTypeId",
                table: "GoodsDetails");

            migrationBuilder.DropColumn(
                name: "OtherGoodsCategoryName",
                table: "GoodsDetails");

            migrationBuilder.DropColumn(
                name: "OtherUnitOfMeasureName",
                table: "GoodsDetails");

            migrationBuilder.DropColumn(
                name: "RoutPointId",
                table: "GoodsDetails");

            migrationBuilder.DropColumn(
                name: "FatherId",
                table: "GoodCategories");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "GoodCategories");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Facilities");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Facilities");

            migrationBuilder.DropColumn(
                name: "DocumentRelatedWithId",
                table: "DocumentTypes");

            migrationBuilder.DropColumn(
                name: "TemplateContentType",
                table: "DocumentTypes");

            migrationBuilder.DropColumn(
                name: "TemplateId",
                table: "DocumentTypes");

            migrationBuilder.DropColumn(
                name: "TemplateName",
                table: "DocumentTypes");

            migrationBuilder.DropColumn(
                name: "OtherDocumentTypeName",
                table: "DocumentFiles");

            migrationBuilder.DropColumn(
                name: "ShippingRequestTripId",
                table: "DocumentFiles");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "AccountNumber",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "AccountNumber",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "Balance",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "ContractNumber",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "CreditBalance",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "MobileNo",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "ReservedBalance",
                table: "AbpTenants");

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "Vases",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "TrucksTypes",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "ShippingRequestVases",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<long>(
                name: "TrucksTypeId",
                table: "ShippingRequests",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "GoodCategoryId",
                table: "ShippingRequests",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "AssignedTrailerId",
                table: "ShippingRequests",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsRejected",
                table: "ShippingRequests",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RouteId",
                table: "ShippingRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ShippingRequestBidStatusId",
                table: "ShippingRequests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShippingRequestStatusId",
                table: "ShippingRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "StageOneFinish",
                table: "ShippingRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "StageThreeFinish",
                table: "ShippingRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "StageTowFinish",
                table: "ShippingRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<double>(
                name: "price",
                table: "ShippingRequestBids",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<long>(
                name: "ShippingRequestId",
                table: "RoutSteps",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<long>(
                name: "AssignedTruckId",
                table: "RoutSteps",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "AssignedDriverUserId",
                table: "RoutSteps",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "AssignedTrailerId",
                table: "RoutSteps",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "DestinationCityId",
                table: "RoutSteps",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DestinationFacilityId",
                table: "RoutSteps",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "GoodsDetailId",
                table: "RoutSteps",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Latitude",
                table: "RoutSteps",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Longitude",
                table: "RoutSteps",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OriginCityId",
                table: "RoutSteps",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PickingTypeId",
                table: "RoutSteps",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "SourceFacilityId",
                table: "RoutSteps",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TrailerTypeId",
                table: "RoutSteps",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TrucksTypeId",
                table: "RoutSteps",
                type: "bigint",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "OriginCityId",
                table: "Routes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "DestinationCityId",
                table: "Routes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<string>(
                name: "Adress",
                table: "Ports",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Latitude",
                table: "Ports",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Longitude",
                table: "Ports",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "PlateTypes",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Weight",
                table: "GoodsDetails",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(double),
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<int>(
                name: "GoodCategoryId",
                table: "GoodsDetails",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "GoodsDetails",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Quantity",
                table: "GoodsDetails",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "GoodsDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "GoodCategories",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Adress",
                table: "Facilities",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Latitude",
                table: "Facilities",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Longitude",
                table: "Facilities",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<long>(
                name: "DocumentTypeId",
                table: "DocumentFiles",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Latitude",
                table: "Cities",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Longitude",
                table: "Cities",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PickingTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PickingTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShippingRequestBidStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingRequestBidStatuses", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestVases_TenantId",
                table: "ShippingRequestVases",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequests_AssignedTrailerId",
                table: "ShippingRequests",
                column: "AssignedTrailerId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequests_RouteId",
                table: "ShippingRequests",
                column: "RouteId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequests_ShippingRequestBidStatusId",
                table: "ShippingRequests",
                column: "ShippingRequestBidStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequests_ShippingRequestStatusId",
                table: "ShippingRequests",
                column: "ShippingRequestStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutSteps_AssignedTrailerId",
                table: "RoutSteps",
                column: "AssignedTrailerId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutSteps_DestinationCityId",
                table: "RoutSteps",
                column: "DestinationCityId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutSteps_DestinationFacilityId",
                table: "RoutSteps",
                column: "DestinationFacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutSteps_GoodsDetailId",
                table: "RoutSteps",
                column: "GoodsDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutSteps_OriginCityId",
                table: "RoutSteps",
                column: "OriginCityId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutSteps_PickingTypeId",
                table: "RoutSteps",
                column: "PickingTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutSteps_SourceFacilityId",
                table: "RoutSteps",
                column: "SourceFacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutSteps_TrailerTypeId",
                table: "RoutSteps",
                column: "TrailerTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutSteps_TrucksTypeId",
                table: "RoutSteps",
                column: "TrucksTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_TenantId",
                table: "Routes",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsDetails_TenantId",
                table: "GoodsDetails",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentFiles_DocumentTypes_DocumentTypeId",
                table: "DocumentFiles",
                column: "DocumentTypeId",
                principalTable: "DocumentTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GoodsDetails_GoodCategories_GoodCategoryId",
                table: "GoodsDetails",
                column: "GoodCategoryId",
                principalTable: "GoodCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Routes_Cities_DestinationCityId",
                table: "Routes",
                column: "DestinationCityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Routes_Cities_OriginCityId",
                table: "Routes",
                column: "OriginCityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoutSteps_AbpUsers_AssignedDriverUserId",
                table: "RoutSteps",
                column: "AssignedDriverUserId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoutSteps_Trailers_AssignedTrailerId",
                table: "RoutSteps",
                column: "AssignedTrailerId",
                principalTable: "Trailers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoutSteps_Trucks_AssignedTruckId",
                table: "RoutSteps",
                column: "AssignedTruckId",
                principalTable: "Trucks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoutSteps_Cities_DestinationCityId",
                table: "RoutSteps",
                column: "DestinationCityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoutSteps_Facilities_DestinationFacilityId",
                table: "RoutSteps",
                column: "DestinationFacilityId",
                principalTable: "Facilities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoutSteps_GoodsDetails_GoodsDetailId",
                table: "RoutSteps",
                column: "GoodsDetailId",
                principalTable: "GoodsDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoutSteps_Cities_OriginCityId",
                table: "RoutSteps",
                column: "OriginCityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoutSteps_PickingTypes_PickingTypeId",
                table: "RoutSteps",
                column: "PickingTypeId",
                principalTable: "PickingTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoutSteps_ShippingRequests_ShippingRequestId",
                table: "RoutSteps",
                column: "ShippingRequestId",
                principalTable: "ShippingRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoutSteps_Facilities_SourceFacilityId",
                table: "RoutSteps",
                column: "SourceFacilityId",
                principalTable: "Facilities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoutSteps_TrailerTypes_TrailerTypeId",
                table: "RoutSteps",
                column: "TrailerTypeId",
                principalTable: "TrailerTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoutSteps_TrucksTypes_TrucksTypeId",
                table: "RoutSteps",
                column: "TrucksTypeId",
                principalTable: "TrucksTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequests_Trailers_AssignedTrailerId",
                table: "ShippingRequests",
                column: "AssignedTrailerId",
                principalTable: "Trailers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequests_GoodCategories_GoodCategoryId",
                table: "ShippingRequests",
                column: "GoodCategoryId",
                principalTable: "GoodCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequests_Routes_RouteId",
                table: "ShippingRequests",
                column: "RouteId",
                principalTable: "Routes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequests_ShippingRequestBidStatuses_ShippingRequestBidStatusId",
                table: "ShippingRequests",
                column: "ShippingRequestBidStatusId",
                principalTable: "ShippingRequestBidStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequests_ShippingRequestStatuses_ShippingRequestStatusId",
                table: "ShippingRequests",
                column: "ShippingRequestStatusId",
                principalTable: "ShippingRequestStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequests_TrucksTypes_TrucksTypeId",
                table: "ShippingRequests",
                column: "TrucksTypeId",
                principalTable: "TrucksTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
