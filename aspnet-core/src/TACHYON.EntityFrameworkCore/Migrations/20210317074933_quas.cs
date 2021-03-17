using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

namespace TACHYON.Migrations
{
    public partial class quas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "FK_ShippingRequests_ShippingRequestBidStatuses_ShippingRequestBidStatusId",
                table: "ShippingRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_ShippingRequestStatuses_ShippingRequestStatusId",
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
                name: "IX_GoodsDetails_TenantId",
                table: "GoodsDetails");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "ShippingRequestVases");

            migrationBuilder.DropColumn(
                name: "AssignedTrailerId",
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
                name: "Name",
                table: "GoodsDetails");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "GoodsDetails");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "GoodsDetails");

            migrationBuilder.DropColumn(
                name: "Adress",
                table: "Facilities");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Facilities");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Facilities");

            migrationBuilder.AddColumn<int>(
                name: "NumberOfTrips",
                table: "ShippingRequestVases",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<byte>(
                name: "BidStatus",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndTripDate",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCarrierHaveInvoice",
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
                name: "PackingTypeId",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.AddColumn<long>(
                name: "RoutPointId",
                table: "GoodsDetails",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "FatherId",
                table: "GoodCategories",
                nullable: true);

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

            migrationBuilder.AddColumn<decimal>(
                name: "Balance",
                table: "AbpTenants",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CreditBalance",
                table: "AbpTenants",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ReservedBalance",
                table: "AbpTenants",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "BalanceRecharges",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BalanceRecharges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BalanceRecharges_AbpTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    CreditLimit = table.Column<double>(nullable: false),
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
                    TenantId = table.Column<int>(nullable: false),
                    RequestId = table.Column<long>(nullable: false),
                    TotalAmount = table.Column<decimal>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    VatAmount = table.Column<decimal>(nullable: false),
                    TaxVat = table.Column<decimal>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoicesProforma", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoicesProforma_ShippingRequests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "ShippingRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShippingRequestCausesAccidents",
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
                    DisplayName = table.Column<string>(maxLength: 60, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingRequestCausesAccidents", x => x.Id);
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
                    StartTripDate = table.Column<DateTime>(nullable: false),
                    EndTripDate = table.Column<DateTime>(nullable: false),
                    StartWorking = table.Column<DateTime>(nullable: true),
                    EndWorking = table.Column<DateTime>(nullable: true),
                    Status = table.Column<byte>(nullable: false),
                    AssignedDriverUserId = table.Column<long>(nullable: true),
                    AssignedTruckId = table.Column<long>(nullable: true),
                    ShippingRequestId = table.Column<long>(nullable: false),
                    OriginFacilityId = table.Column<long>(nullable: true),
                    DestinationFacilityId = table.Column<long>(nullable: true)
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
                        name: "FK_ShippingRequestTrips_ShippingRequests_ShippingRequestId",
                        column: x => x.ShippingRequestId,
                        principalTable: "ShippingRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "TransactionsChannels",
                columns: table => new
                {
                    Id = table.Column<byte>(nullable: false),
                    Channel = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionsChannels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TripStatuses",
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
                    DisplayName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TripStatuses", x => x.Id);
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
                    IsDemand = table.Column<bool>(nullable: false),
                    Note = table.Column<string>(nullable: true),
                    AmountWithTaxVat = table.Column<decimal>(nullable: false),
                    VatAmount = table.Column<decimal>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    TaxVat = table.Column<decimal>(nullable: false),
                    BinaryObjectId = table.Column<Guid>(nullable: true),
                    DemandFileName = table.Column<string>(nullable: true),
                    DemandFileContentType = table.Column<string>(nullable: true),
                    IsClaim = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupPeriods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupPeriods_InvoicePeriods_PeriodId",
                        column: x => x.PeriodId,
                        principalTable: "InvoicePeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupPeriods_AbpTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    TenantId = table.Column<int>(nullable: false),
                    PeriodId = table.Column<int>(nullable: false),
                    DueDate = table.Column<DateTime>(nullable: false),
                    IsPaid = table.Column<bool>(nullable: false),
                    Note = table.Column<string>(nullable: true),
                    AmountWithTaxVat = table.Column<decimal>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    VatAmount = table.Column<decimal>(nullable: false),
                    TaxVat = table.Column<decimal>(nullable: false),
                    IsAccountReceivable = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invoices_InvoicePeriods_PeriodId",
                        column: x => x.PeriodId,
                        principalTable: "InvoicePeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Invoices_AbpTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    DisplayName = table.Column<string>(nullable: true),
                    PickingType = table.Column<byte>(nullable: false),
                    FacilityId = table.Column<long>(nullable: false),
                    ShippingRequestTripId = table.Column<int>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    StartTime = table.Column<DateTime>(nullable: true),
                    EndTime = table.Column<DateTime>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    IsComplete = table.Column<bool>(nullable: false),
                    DocumentId = table.Column<Guid>(nullable: true),
                    DocumentName = table.Column<string>(nullable: true),
                    DocumentContentType = table.Column<string>(nullable: true),
                    Rating = table.Column<int>(nullable: true),
                    ReceiverId = table.Column<int>(nullable: true),
                    ReceiverFullName = table.Column<string>(nullable: true),
                    ReceiverPhoneNumber = table.Column<string>(nullable: true),
                    ReceiverEmailAddress = table.Column<string>(nullable: true),
                    ReceiverCardIdNumber = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoutPoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoutPoints_Facilities_FacilityId",
                        column: x => x.FacilityId,
                        principalTable: "Facilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoutPoints_ShippingRequestTrips_ShippingRequestTripId",
                        column: x => x.ShippingRequestTripId,
                        principalTable: "ShippingRequestTrips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    ShippingRequestTripId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingRequestTripVases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingRequestTripVases_ShippingRequestTrips_ShippingRequestTripId",
                        column: x => x.ShippingRequestTripId,
                        principalTable: "ShippingRequestTrips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShippingRequestTripVases_ShippingRequestVases_ShippingRequestVasId",
                        column: x => x.ShippingRequestVasId,
                        principalTable: "ShippingRequestVases",
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
                        name: "FK_Transactions_TransactionsChannels_ChannelId",
                        column: x => x.ChannelId,
                        principalTable: "TransactionsChannels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transactions_AbpTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AbpTenants",
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
                        onDelete: ReferentialAction.Cascade);
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
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupPeriodsInvoices_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceShippingRequests",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestId = table.Column<long>(nullable: false),
                    InvoiceId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceShippingRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoiceShippingRequests_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvoiceShippingRequests_ShippingRequests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "ShippingRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoutePointTransitions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FromPointId = table.Column<long>(nullable: false),
                    ToPointId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoutePointTransitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoutePointTransitions_RoutPoints_FromPointId",
                        column: x => x.FromPointId,
                        principalTable: "RoutPoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoutePointTransitions_RoutPoints_ToPointId",
                        column: x => x.ToPointId,
                        principalTable: "RoutPoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequests_PackingTypeId",
                table: "ShippingRequests",
                column: "PackingTypeId");

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
                name: "IX_GoodsDetails_RoutPointId",
                table: "GoodsDetails",
                column: "RoutPointId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodCategories_FatherId",
                table: "GoodCategories",
                column: "FatherId");

            migrationBuilder.CreateIndex(
                name: "IX_BalanceRecharges_TenantId",
                table: "BalanceRecharges",
                column: "TenantId");

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
                name: "IX_Invoices_PeriodId",
                table: "Invoices",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_TenantId",
                table: "Invoices",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceShippingRequests_InvoiceId",
                table: "InvoiceShippingRequests",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceShippingRequests_RequestId",
                table: "InvoiceShippingRequests",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoicesProforma_RequestId",
                table: "InvoicesProforma",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoicesProforma_TenantId",
                table: "InvoicesProforma",
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
                name: "IX_RoutePointTransitions_FromPointId",
                table: "RoutePointTransitions",
                column: "FromPointId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutePointTransitions_ToPointId",
                table: "RoutePointTransitions",
                column: "ToPointId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutPoints_FacilityId",
                table: "RoutPoints",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutPoints_ShippingRequestTripId",
                table: "RoutPoints",
                column: "ShippingRequestTripId");

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
                name: "IX_ShippingRequestTrips_ShippingRequestId",
                table: "ShippingRequestTrips",
                column: "ShippingRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTripVases_ShippingRequestTripId",
                table: "ShippingRequestTripVases",
                column: "ShippingRequestTripId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTripVases_ShippingRequestVasId",
                table: "ShippingRequestTripVases",
                column: "ShippingRequestVasId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_ChannelId",
                table: "Transactions",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_TenantId",
                table: "Transactions",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_GoodCategories_GoodCategories_FatherId",
                table: "GoodCategories",
                column: "FatherId",
                principalTable: "GoodCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GoodsDetails_GoodCategories_GoodCategoryId",
                table: "GoodsDetails",
                column: "GoodCategoryId",
                principalTable: "GoodCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
                onDelete: ReferentialAction.Cascade);

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
                name: "FK_ShippingRequests_PackingTypes_PackingTypeId",
                table: "ShippingRequests",
                column: "PackingTypeId",
                principalTable: "PackingTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequests_ShippingTypes_ShippingTypeId",
                table: "ShippingRequests",
                column: "ShippingTypeId",
                principalTable: "ShippingTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GoodCategories_GoodCategories_FatherId",
                table: "GoodCategories");

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
                name: "FK_ShippingRequests_PackingTypes_PackingTypeId",
                table: "ShippingRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_ShippingTypes_ShippingTypeId",
                table: "ShippingRequests");

            migrationBuilder.DropTable(
                name: "BalanceRecharges");

            migrationBuilder.DropTable(
                name: "GroupPeriodsInvoices");

            migrationBuilder.DropTable(
                name: "GroupShippingRequests");

            migrationBuilder.DropTable(
                name: "InvoiceShippingRequests");

            migrationBuilder.DropTable(
                name: "InvoicesProforma");

            migrationBuilder.DropTable(
                name: "PackingTypes");

            migrationBuilder.DropTable(
                name: "Receivers");

            migrationBuilder.DropTable(
                name: "RoutePointTransitions");

            migrationBuilder.DropTable(
                name: "ShippingRequestCausesAccidents");

            migrationBuilder.DropTable(
                name: "ShippingRequestTripVases");

            migrationBuilder.DropTable(
                name: "ShippingTypes");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "TripStatuses");

            migrationBuilder.DropTable(
                name: "GroupPeriods");

            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropTable(
                name: "RoutPoints");

            migrationBuilder.DropTable(
                name: "TransactionsChannels");

            migrationBuilder.DropTable(
                name: "InvoicePeriods");

            migrationBuilder.DropTable(
                name: "ShippingRequestTrips");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequests_PackingTypeId",
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
                name: "IX_GoodsDetails_RoutPointId",
                table: "GoodsDetails");

            migrationBuilder.DropIndex(
                name: "IX_GoodCategories_FatherId",
                table: "GoodCategories");

            migrationBuilder.DropColumn(
                name: "NumberOfTrips",
                table: "ShippingRequestVases");

            migrationBuilder.DropColumn(
                name: "BidStatus",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "EndTripDate",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "IsCarrierHaveInvoice",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "IsPrePayed",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "IsShipperHaveInvoice",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "NumberOfPacking",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "NumberOfTrips",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "PackingTypeId",
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
                name: "TotalWeight",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "TotalsTripsAddByShippier",
                table: "ShippingRequests");

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
                name: "Amount",
                table: "GoodsDetails");

            migrationBuilder.DropColumn(
                name: "RoutPointId",
                table: "GoodsDetails");

            migrationBuilder.DropColumn(
                name: "FatherId",
                table: "GoodCategories");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Facilities");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Facilities");

            migrationBuilder.DropColumn(
                name: "Balance",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "CreditBalance",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "ReservedBalance",
                table: "AbpTenants");

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "ShippingRequestVases",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "AssignedTrailerId",
                table: "ShippingRequests",
                type: "bigint",
                nullable: true);

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
                name: "IX_GoodsDetails_TenantId",
                table: "GoodsDetails",
                column: "TenantId");

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
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoutSteps_Trailers_AssignedTrailerId",
                table: "RoutSteps",
                column: "AssignedTrailerId",
                principalTable: "Trailers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoutSteps_Trucks_AssignedTruckId",
                table: "RoutSteps",
                column: "AssignedTruckId",
                principalTable: "Trucks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
                onDelete: ReferentialAction.Cascade);

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
                onDelete: ReferentialAction.Cascade);
        }
    }
}
