using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class release_2_4_11 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentTypes_DocumentsEntities_DocumentsEntityId",
                table: "DocumentTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_Penalties_AbpTenants_TenantId",
                table: "Penalties");

            migrationBuilder.DropTable(
                name: "DocumentsEntities");

            //migrationBuilder.DropIndex(
            //    name: "IX_DocumentTypes_DocumentsEntityId",
            //    table: "DocumentTypes");

            migrationBuilder.AddColumn<int>(
                name: "CarrierActorId",
                table: "Trucks",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DriverUserId",
                table: "Trucks",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InternalTruckId",
                table: "Trucks",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AcceptanceDate",
                table: "SubmitInvoices",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentDate",
                table: "SubmitInvoices",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SubmittedDate",
                table: "SubmitInvoices",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ActorCarrierPriceId",
                table: "ShippingRequestVases",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ActorShipperPriceId",
                table: "ShippingRequestVases",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ActorInvoiceId",
                table: "ShippingRequestTrips",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ActorSubmitInvoiceId",
                table: "ShippingRequestTrips",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActorCarrierHaveInvoice",
                table: "ShippingRequestTrips",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActorShipperHaveInvoice",
                table: "ShippingRequestTrips",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfDrops",
                table: "ShippingRequestTrips",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<byte>(
                name: "RouteType",
                table: "ShippingRequestTrips",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ActorCarrierPriceId",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ActorShipperPriceId",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CarrierActorId",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "DedicatedKPI",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "ExpectedMileage",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfTrucks",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RentalDuration",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<byte>(
                name: "RentalDurationUnit",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RentalEndDate",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RentalStartDate",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ServiceAreaNotes",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShipperActorId",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "ShippingRequestFlag",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<int>(
                name: "CarrierActorId",
                table: "PriceOffers",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TenantId",
                table: "Penalties",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "InvoiceFlag",
                table: "Penalties",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReferenceNumber",
                table: "Penalties",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentDate",
                table: "Invoices",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Invoices",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceNoteReferenceNumber",
                table: "InvoiceNotes",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShipperActorId",
                table: "Facilities",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Flag",
                table: "DocumentTypes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ActorId",
                table: "DocumentFiles",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CarrierActorId",
                table: "AbpUsers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ActorCarrierPrices",
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
                    ShippingRequestId = table.Column<long>(nullable: true),
                    ShippingRequestVasId = table.Column<long>(nullable: true),
                    IsActorCarrierHaveInvoice = table.Column<bool>(nullable: false),
                    SubTotalAmount = table.Column<decimal>(nullable: true),
                    VatAmount = table.Column<decimal>(nullable: true),
                    TaxVat = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActorCarrierPrices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActorCarrierPrices_ShippingRequests_ShippingRequestId",
                        column: x => x.ShippingRequestId,
                        principalTable: "ShippingRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ActorCarrierPrices_ShippingRequestVases_ShippingRequestVasId",
                        column: x => x.ShippingRequestVasId,
                        principalTable: "ShippingRequestVases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Actors",
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
                    CompanyName = table.Column<string>(maxLength: 255, nullable: false),
                    ActorType = table.Column<byte>(nullable: false),
                    MoiNumber = table.Column<string>(nullable: false),
                    Address = table.Column<string>(nullable: false),
                    Logo = table.Column<string>(nullable: true),
                    MobileNumber = table.Column<string>(nullable: false),
                    Email = table.Column<string>(nullable: false),
                    OrganizationUnitId = table.Column<long>(nullable: false),
                    InvoiceDueDays = table.Column<int>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: true),
                    CR = table.Column<string>(nullable: true),
                    VatCertificate = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ActorShipperPrices",
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
                    ShippingRequestId = table.Column<long>(nullable: true),
                    ShippingRequestVasId = table.Column<long>(nullable: true),
                    TotalAmountWithCommission = table.Column<decimal>(nullable: true),
                    SubTotalAmountWithCommission = table.Column<decimal>(nullable: true),
                    VatAmountWithCommission = table.Column<decimal>(nullable: true),
                    TaxVat = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActorShipperPrices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActorShipperPrices_ShippingRequests_ShippingRequestId",
                        column: x => x.ShippingRequestId,
                        principalTable: "ShippingRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ActorShipperPrices_ShippingRequestVases_ShippingRequestVasId",
                        column: x => x.ShippingRequestVasId,
                        principalTable: "ShippingRequestVases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DedicatedDynamicInvoices",
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
                    InvoiceAccountType = table.Column<byte>(nullable: false),
                    ShippingRequestId = table.Column<long>(nullable: false),
                    SubTotalAmount = table.Column<decimal>(nullable: false),
                    TaxVat = table.Column<decimal>(nullable: false),
                    VatAmount = table.Column<decimal>(nullable: false),
                    TotalAmount = table.Column<decimal>(nullable: false),
                    InvoiceId = table.Column<long>(nullable: true),
                    SubmitInvoiceId = table.Column<long>(nullable: true),
                    Notes = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DedicatedDynamicInvoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DedicatedDynamicInvoices_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DedicatedDynamicInvoices_ShippingRequests_ShippingRequestId",
                        column: x => x.ShippingRequestId,
                        principalTable: "ShippingRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DedicatedDynamicInvoices_SubmitInvoices_SubmitInvoiceId",
                        column: x => x.SubmitInvoiceId,
                        principalTable: "SubmitInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DedicatedDynamicInvoices_AbpTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DedicatedShippingRequestDrivers",
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
                    ShippingRequestId = table.Column<long>(nullable: false),
                    DriverUserId = table.Column<long>(nullable: false),
                    Status = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DedicatedShippingRequestDrivers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DedicatedShippingRequestDrivers_AbpUsers_DriverUserId",
                        column: x => x.DriverUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DedicatedShippingRequestDrivers_ShippingRequests_ShippingRequestId",
                        column: x => x.ShippingRequestId,
                        principalTable: "ShippingRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DedicatedShippingRequestTrucks",
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
                    ShippingRequestId = table.Column<long>(nullable: false),
                    TruckId = table.Column<long>(nullable: false),
                    Status = table.Column<byte>(nullable: false),
                    KPI = table.Column<double>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DedicatedShippingRequestTrucks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DedicatedShippingRequestTrucks_ShippingRequests_ShippingRequestId",
                        column: x => x.ShippingRequestId,
                        principalTable: "ShippingRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DedicatedShippingRequestTrucks_Trucks_TruckId",
                        column: x => x.TruckId,
                        principalTable: "Trucks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PenaltyItems",
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
                    PenaltyId = table.Column<int>(nullable: false),
                    ShippingRequestTripId = table.Column<int>(nullable: true),
                    ItemPrice = table.Column<decimal>(nullable: false),
                    ItemTotalAmountPostVat = table.Column<decimal>(nullable: false),
                    VatAmount = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PenaltyItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PenaltyItems_Penalties_PenaltyId",
                        column: x => x.PenaltyId,
                        principalTable: "Penalties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PenaltyItems_ShippingRequestTrips_ShippingRequestTripId",
                        column: x => x.ShippingRequestTripId,
                        principalTable: "ShippingRequestTrips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PricePackageProposals",
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
                    ProposalName = table.Column<string>(nullable: true),
                    ScopeOverview = table.Column<string>(nullable: true),
                    ProposalDate = table.Column<DateTime>(nullable: true),
                    ShipperId = table.Column<int>(nullable: false),
                    Notes = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    ProposalFileId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PricePackageProposals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PricePackageProposals_AbpTenants_ShipperId",
                        column: x => x.ShipperId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceAreas",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    CityId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceAreas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceAreas_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShippingRequestDestinationCities",
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
                    ShippingRequestId = table.Column<long>(nullable: false),
                    CityId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingRequestDestinationCities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingRequestDestinationCities_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShippingRequestDestinationCities_ShippingRequests_ShippingRequestId",
                        column: x => x.ShippingRequestId,
                        principalTable: "ShippingRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ActorInvoices",
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
                    InvoiceNumber = table.Column<string>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    DueDate = table.Column<DateTime>(nullable: false),
                    IsPaid = table.Column<bool>(nullable: false),
                    Note = table.Column<string>(nullable: true),
                    SubTotalAmount = table.Column<decimal>(nullable: false),
                    TotalAmount = table.Column<decimal>(nullable: false),
                    VatAmount = table.Column<decimal>(nullable: false),
                    TaxVat = table.Column<decimal>(nullable: false),
                    ShipperActorId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActorInvoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActorInvoices_Actors_ShipperActorId",
                        column: x => x.ShipperActorId,
                        principalTable: "Actors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ActorInvoices_AbpTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ActorSubmitInvoices",
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
                    ReferencNumber = table.Column<string>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    DueDate = table.Column<DateTime>(nullable: false),
                    SubTotalAmount = table.Column<decimal>(nullable: false),
                    TotalAmount = table.Column<decimal>(nullable: false),
                    VatAmount = table.Column<decimal>(nullable: false),
                    TaxVat = table.Column<decimal>(nullable: false),
                    CarrierActorId = table.Column<int>(nullable: true),
                    Status = table.Column<byte>(nullable: false),
                    DocumentId = table.Column<Guid>(nullable: true),
                    DocumentName = table.Column<string>(nullable: true),
                    DocumentContentType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActorSubmitInvoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActorSubmitInvoices_Actors_CarrierActorId",
                        column: x => x.CarrierActorId,
                        principalTable: "Actors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ActorSubmitInvoices_AbpTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DedicatedDynamicInvoiceItems",
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
                    DedicatedDynamicInvoiceId = table.Column<long>(nullable: false),
                    DedicatedShippingRequestTruckId = table.Column<long>(nullable: false),
                    NumberOfDays = table.Column<int>(nullable: false),
                    PricePerDay = table.Column<decimal>(nullable: false),
                    WorkingDayType = table.Column<byte>(nullable: false),
                    ItemSubTotalAmount = table.Column<decimal>(nullable: false),
                    VatAmount = table.Column<decimal>(nullable: false),
                    TaxVat = table.Column<decimal>(nullable: false),
                    ItemTotalAmount = table.Column<decimal>(nullable: false),
                    Remarks = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DedicatedDynamicInvoiceItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DedicatedDynamicInvoiceItems_DedicatedDynamicInvoices_DedicatedDynamicInvoiceId",
                        column: x => x.DedicatedDynamicInvoiceId,
                        principalTable: "DedicatedDynamicInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DedicatedDynamicInvoiceItems_DedicatedShippingRequestTrucks_DedicatedShippingRequestTruckId",
                        column: x => x.DedicatedShippingRequestTruckId,
                        principalTable: "DedicatedShippingRequestTrucks",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DedicatedShippingRequestTruckAttendances",
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
                    DedicatedShippingRequestTruckId = table.Column<long>(nullable: false),
                    AttendanceDate = table.Column<DateTime>(nullable: false),
                    AttendaceStatus = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DedicatedShippingRequestTruckAttendances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DedicatedShippingRequestTruckAttendances_DedicatedShippingRequestTrucks_DedicatedShippingRequestTruckId",
                        column: x => x.DedicatedShippingRequestTruckId,
                        principalTable: "DedicatedShippingRequestTrucks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PricePackageAppendixes",
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
                    ContractName = table.Column<string>(nullable: true),
                    ContractNumber = table.Column<int>(nullable: false),
                    Version = table.Column<int>(nullable: false),
                    AppendixDate = table.Column<DateTime>(nullable: true),
                    ScopeOverview = table.Column<string>(nullable: true),
                    Notes = table.Column<string>(nullable: true),
                    ProposalId = table.Column<int>(nullable: false),
                    AppendixFileId = table.Column<Guid>(nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PricePackageAppendixes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PricePackageAppendixes_PricePackageProposals_ProposalId",
                        column: x => x.ProposalId,
                        principalTable: "PricePackageProposals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TmsPricePackages",
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
                    OriginCityId = table.Column<int>(nullable: true),
                    DestinationCityId = table.Column<int>(nullable: true),
                    ShipperId = table.Column<int>(nullable: true),
                    RouteType = table.Column<byte>(nullable: false),
                    DirectRequestPrice = table.Column<decimal>(nullable: false),
                    TachyonManagePrice = table.Column<decimal>(nullable: false),
                    DirectRequestCommission = table.Column<decimal>(nullable: false),
                    TachyonManageCommission = table.Column<decimal>(nullable: false),
                    DirectRequestTotalPrice = table.Column<decimal>(nullable: false),
                    TachyonManageTotalPrice = table.Column<decimal>(nullable: false),
                    CommissionType = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    ProposalId = table.Column<int>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TmsPricePackages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TmsPricePackages_Cities_DestinationCityId",
                        column: x => x.DestinationCityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TmsPricePackages_Cities_OriginCityId",
                        column: x => x.OriginCityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TmsPricePackages_PricePackageProposals_ProposalId",
                        column: x => x.ProposalId,
                        principalTable: "PricePackageProposals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TmsPricePackages_AbpTenants_ShipperId",
                        column: x => x.ShipperId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TmsPricePackages_AbpTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TmsPricePackages_TransportTypes_TransportTypeId",
                        column: x => x.TransportTypeId,
                        principalTable: "TransportTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TmsPricePackages_TrucksTypes_TrucksTypeId",
                        column: x => x.TrucksTypeId,
                        principalTable: "TrucksTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Trucks_CarrierActorId",
                table: "Trucks",
                column: "CarrierActorId");

            migrationBuilder.CreateIndex(
                name: "IX_Trucks_DriverUserId",
                table: "Trucks",
                column: "DriverUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestVases_ActorCarrierPriceId",
                table: "ShippingRequestVases",
                column: "ActorCarrierPriceId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestVases_ActorShipperPriceId",
                table: "ShippingRequestVases",
                column: "ActorShipperPriceId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTrips_ActorInvoiceId",
                table: "ShippingRequestTrips",
                column: "ActorInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTrips_ActorSubmitInvoiceId",
                table: "ShippingRequestTrips",
                column: "ActorSubmitInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequests_ActorCarrierPriceId",
                table: "ShippingRequests",
                column: "ActorCarrierPriceId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequests_ActorShipperPriceId",
                table: "ShippingRequests",
                column: "ActorShipperPriceId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequests_CarrierActorId",
                table: "ShippingRequests",
                column: "CarrierActorId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequests_ShipperActorId",
                table: "ShippingRequests",
                column: "ShipperActorId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceOffers_CarrierActorId",
                table: "PriceOffers",
                column: "CarrierActorId");

            migrationBuilder.CreateIndex(
                name: "IX_Facilities_ShipperActorId",
                table: "Facilities",
                column: "ShipperActorId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentFiles_ActorId",
                table: "DocumentFiles",
                column: "ActorId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpUsers_CarrierActorId",
                table: "AbpUsers",
                column: "CarrierActorId");

            migrationBuilder.CreateIndex(
                name: "IX_ActorCarrierPrices_ShippingRequestId",
                table: "ActorCarrierPrices",
                column: "ShippingRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ActorCarrierPrices_ShippingRequestVasId",
                table: "ActorCarrierPrices",
                column: "ShippingRequestVasId");

            migrationBuilder.CreateIndex(
                name: "IX_ActorInvoices_ShipperActorId",
                table: "ActorInvoices",
                column: "ShipperActorId");

            migrationBuilder.CreateIndex(
                name: "IX_ActorInvoices_TenantId",
                table: "ActorInvoices",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Actors_TenantId",
                table: "Actors",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ActorShipperPrices_ShippingRequestId",
                table: "ActorShipperPrices",
                column: "ShippingRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ActorShipperPrices_ShippingRequestVasId",
                table: "ActorShipperPrices",
                column: "ShippingRequestVasId");

            migrationBuilder.CreateIndex(
                name: "IX_ActorSubmitInvoices_CarrierActorId",
                table: "ActorSubmitInvoices",
                column: "CarrierActorId");

            migrationBuilder.CreateIndex(
                name: "IX_ActorSubmitInvoices_TenantId",
                table: "ActorSubmitInvoices",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_DedicatedDynamicInvoiceItems_DedicatedDynamicInvoiceId",
                table: "DedicatedDynamicInvoiceItems",
                column: "DedicatedDynamicInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_DedicatedDynamicInvoiceItems_DedicatedShippingRequestTruckId",
                table: "DedicatedDynamicInvoiceItems",
                column: "DedicatedShippingRequestTruckId");

            migrationBuilder.CreateIndex(
                name: "IX_DedicatedDynamicInvoices_InvoiceId",
                table: "DedicatedDynamicInvoices",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_DedicatedDynamicInvoices_ShippingRequestId",
                table: "DedicatedDynamicInvoices",
                column: "ShippingRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_DedicatedDynamicInvoices_SubmitInvoiceId",
                table: "DedicatedDynamicInvoices",
                column: "SubmitInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_DedicatedDynamicInvoices_TenantId",
                table: "DedicatedDynamicInvoices",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_DedicatedShippingRequestDrivers_DriverUserId",
                table: "DedicatedShippingRequestDrivers",
                column: "DriverUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DedicatedShippingRequestDrivers_ShippingRequestId",
                table: "DedicatedShippingRequestDrivers",
                column: "ShippingRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_DedicatedShippingRequestTruckAttendances_DedicatedShippingRequestTruckId",
                table: "DedicatedShippingRequestTruckAttendances",
                column: "DedicatedShippingRequestTruckId");

            migrationBuilder.CreateIndex(
                name: "IX_DedicatedShippingRequestTrucks_ShippingRequestId",
                table: "DedicatedShippingRequestTrucks",
                column: "ShippingRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_DedicatedShippingRequestTrucks_TruckId",
                table: "DedicatedShippingRequestTrucks",
                column: "TruckId");

            migrationBuilder.CreateIndex(
                name: "IX_PenaltyItems_PenaltyId",
                table: "PenaltyItems",
                column: "PenaltyId");

            migrationBuilder.CreateIndex(
                name: "IX_PenaltyItems_ShippingRequestTripId",
                table: "PenaltyItems",
                column: "ShippingRequestTripId");

            migrationBuilder.CreateIndex(
                name: "IX_PricePackageAppendixes_ProposalId",
                table: "PricePackageAppendixes",
                column: "ProposalId");

            migrationBuilder.CreateIndex(
                name: "IX_PricePackageProposals_ShipperId",
                table: "PricePackageProposals",
                column: "ShipperId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceAreas_CityId",
                table: "ServiceAreas",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestDestinationCities_CityId",
                table: "ShippingRequestDestinationCities",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestDestinationCities_ShippingRequestId",
                table: "ShippingRequestDestinationCities",
                column: "ShippingRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_TmsPricePackages_DestinationCityId",
                table: "TmsPricePackages",
                column: "DestinationCityId");

            migrationBuilder.CreateIndex(
                name: "IX_TmsPricePackages_OriginCityId",
                table: "TmsPricePackages",
                column: "OriginCityId");

            migrationBuilder.CreateIndex(
                name: "IX_TmsPricePackages_ProposalId",
                table: "TmsPricePackages",
                column: "ProposalId");

            migrationBuilder.CreateIndex(
                name: "IX_TmsPricePackages_ShipperId",
                table: "TmsPricePackages",
                column: "ShipperId");

            migrationBuilder.CreateIndex(
                name: "IX_TmsPricePackages_TenantId",
                table: "TmsPricePackages",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TmsPricePackages_TransportTypeId",
                table: "TmsPricePackages",
                column: "TransportTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TmsPricePackages_TrucksTypeId",
                table: "TmsPricePackages",
                column: "TrucksTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_AbpUsers_Actors_CarrierActorId",
                table: "AbpUsers",
                column: "CarrierActorId",
                principalTable: "Actors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentFiles_Actors_ActorId",
                table: "DocumentFiles",
                column: "ActorId",
                principalTable: "Actors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Facilities_Actors_ShipperActorId",
                table: "Facilities",
                column: "ShipperActorId",
                principalTable: "Actors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Facilities_AbpTenants_TenantId",
                table: "Facilities",
                column: "TenantId",
                principalTable: "AbpTenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Penalties_AbpTenants_TenantId",
                table: "Penalties",
                column: "TenantId",
                principalTable: "AbpTenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PriceOffers_Actors_CarrierActorId",
                table: "PriceOffers",
                column: "CarrierActorId",
                principalTable: "Actors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Receivers_AbpTenants_TenantId",
                table: "Receivers",
                column: "TenantId",
                principalTable: "AbpTenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequests_ActorCarrierPrices_ActorCarrierPriceId",
                table: "ShippingRequests",
                column: "ActorCarrierPriceId",
                principalTable: "ActorCarrierPrices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequests_ActorShipperPrices_ActorShipperPriceId",
                table: "ShippingRequests",
                column: "ActorShipperPriceId",
                principalTable: "ActorShipperPrices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequests_Actors_CarrierActorId",
                table: "ShippingRequests",
                column: "CarrierActorId",
                principalTable: "Actors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequests_Actors_ShipperActorId",
                table: "ShippingRequests",
                column: "ShipperActorId",
                principalTable: "Actors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequestTrips_ActorInvoices_ActorInvoiceId",
                table: "ShippingRequestTrips",
                column: "ActorInvoiceId",
                principalTable: "ActorInvoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequestTrips_ActorSubmitInvoices_ActorSubmitInvoiceId",
                table: "ShippingRequestTrips",
                column: "ActorSubmitInvoiceId",
                principalTable: "ActorSubmitInvoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequestVases_ActorCarrierPrices_ActorCarrierPriceId",
                table: "ShippingRequestVases",
                column: "ActorCarrierPriceId",
                principalTable: "ActorCarrierPrices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequestVases_ActorShipperPrices_ActorShipperPriceId",
                table: "ShippingRequestVases",
                column: "ActorShipperPriceId",
                principalTable: "ActorShipperPrices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Trucks_Actors_CarrierActorId",
                table: "Trucks",
                column: "CarrierActorId",
                principalTable: "Actors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Trucks_AbpUsers_DriverUserId",
                table: "Trucks",
                column: "DriverUserId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AbpUsers_Actors_CarrierActorId",
                table: "AbpUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_DocumentFiles_Actors_ActorId",
                table: "DocumentFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_Facilities_Actors_ShipperActorId",
                table: "Facilities");

            migrationBuilder.DropForeignKey(
                name: "FK_Facilities_AbpTenants_TenantId",
                table: "Facilities");

            migrationBuilder.DropForeignKey(
                name: "FK_Penalties_AbpTenants_TenantId",
                table: "Penalties");

            migrationBuilder.DropForeignKey(
                name: "FK_PriceOffers_Actors_CarrierActorId",
                table: "PriceOffers");

            migrationBuilder.DropForeignKey(
                name: "FK_Receivers_AbpTenants_TenantId",
                table: "Receivers");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_ActorCarrierPrices_ActorCarrierPriceId",
                table: "ShippingRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_ActorShipperPrices_ActorShipperPriceId",
                table: "ShippingRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_Actors_CarrierActorId",
                table: "ShippingRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_Actors_ShipperActorId",
                table: "ShippingRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestTrips_ActorInvoices_ActorInvoiceId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestTrips_ActorSubmitInvoices_ActorSubmitInvoiceId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestVases_ActorCarrierPrices_ActorCarrierPriceId",
                table: "ShippingRequestVases");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestVases_ActorShipperPrices_ActorShipperPriceId",
                table: "ShippingRequestVases");

            migrationBuilder.DropForeignKey(
                name: "FK_Trucks_Actors_CarrierActorId",
                table: "Trucks");

            migrationBuilder.DropForeignKey(
                name: "FK_Trucks_AbpUsers_DriverUserId",
                table: "Trucks");

            migrationBuilder.DropTable(
                name: "ActorCarrierPrices");

            migrationBuilder.DropTable(
                name: "ActorInvoices");

            migrationBuilder.DropTable(
                name: "ActorShipperPrices");

            migrationBuilder.DropTable(
                name: "ActorSubmitInvoices");

            migrationBuilder.DropTable(
                name: "DedicatedDynamicInvoiceItems");

            migrationBuilder.DropTable(
                name: "DedicatedShippingRequestDrivers");

            migrationBuilder.DropTable(
                name: "DedicatedShippingRequestTruckAttendances");

            migrationBuilder.DropTable(
                name: "PenaltyItems");

            migrationBuilder.DropTable(
                name: "PricePackageAppendixes");

            migrationBuilder.DropTable(
                name: "ServiceAreas");

            migrationBuilder.DropTable(
                name: "ShippingRequestDestinationCities");

            migrationBuilder.DropTable(
                name: "TmsPricePackages");

            migrationBuilder.DropTable(
                name: "Actors");

            migrationBuilder.DropTable(
                name: "DedicatedDynamicInvoices");

            migrationBuilder.DropTable(
                name: "DedicatedShippingRequestTrucks");

            migrationBuilder.DropTable(
                name: "PricePackageProposals");

            migrationBuilder.DropIndex(
                name: "IX_Trucks_CarrierActorId",
                table: "Trucks");

            migrationBuilder.DropIndex(
                name: "IX_Trucks_DriverUserId",
                table: "Trucks");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequestVases_ActorCarrierPriceId",
                table: "ShippingRequestVases");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequestVases_ActorShipperPriceId",
                table: "ShippingRequestVases");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequestTrips_ActorInvoiceId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequestTrips_ActorSubmitInvoiceId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequests_ActorCarrierPriceId",
                table: "ShippingRequests");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequests_ActorShipperPriceId",
                table: "ShippingRequests");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequests_CarrierActorId",
                table: "ShippingRequests");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequests_ShipperActorId",
                table: "ShippingRequests");

            migrationBuilder.DropIndex(
                name: "IX_PriceOffers_CarrierActorId",
                table: "PriceOffers");

            migrationBuilder.DropIndex(
                name: "IX_Facilities_ShipperActorId",
                table: "Facilities");

            migrationBuilder.DropIndex(
                name: "IX_DocumentFiles_ActorId",
                table: "DocumentFiles");

            migrationBuilder.DropIndex(
                name: "IX_AbpUsers_CarrierActorId",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "CarrierActorId",
                table: "Trucks");

            migrationBuilder.DropColumn(
                name: "DriverUserId",
                table: "Trucks");

            migrationBuilder.DropColumn(
                name: "InternalTruckId",
                table: "Trucks");

            migrationBuilder.DropColumn(
                name: "AcceptanceDate",
                table: "SubmitInvoices");

            migrationBuilder.DropColumn(
                name: "PaymentDate",
                table: "SubmitInvoices");

            migrationBuilder.DropColumn(
                name: "SubmittedDate",
                table: "SubmitInvoices");

            migrationBuilder.DropColumn(
                name: "ActorCarrierPriceId",
                table: "ShippingRequestVases");

            migrationBuilder.DropColumn(
                name: "ActorShipperPriceId",
                table: "ShippingRequestVases");

            migrationBuilder.DropColumn(
                name: "ActorInvoiceId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "ActorSubmitInvoiceId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "IsActorCarrierHaveInvoice",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "IsActorShipperHaveInvoice",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "NumberOfDrops",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "RouteType",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "ActorCarrierPriceId",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "ActorShipperPriceId",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "CarrierActorId",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "DedicatedKPI",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "ExpectedMileage",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "NumberOfTrucks",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "RentalDuration",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "RentalDurationUnit",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "RentalEndDate",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "RentalStartDate",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "ServiceAreaNotes",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "ShipperActorId",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "ShippingRequestFlag",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "CarrierActorId",
                table: "PriceOffers");

            migrationBuilder.DropColumn(
                name: "InvoiceFlag",
                table: "Penalties");

            migrationBuilder.DropColumn(
                name: "ReferenceNumber",
                table: "Penalties");

            migrationBuilder.DropColumn(
                name: "PaymentDate",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "InvoiceNoteReferenceNumber",
                table: "InvoiceNotes");

            migrationBuilder.DropColumn(
                name: "ShipperActorId",
                table: "Facilities");

            migrationBuilder.DropColumn(
                name: "Flag",
                table: "DocumentTypes");

            migrationBuilder.DropColumn(
                name: "ActorId",
                table: "DocumentFiles");

            migrationBuilder.DropColumn(
                name: "CarrierActorId",
                table: "AbpUsers");

            migrationBuilder.AlterColumn<int>(
                name: "TenantId",
                table: "Penalties",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "DocumentsEntities",
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
                    table.PrimaryKey("PK_DocumentsEntities", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTypes_DocumentsEntityId",
                table: "DocumentTypes",
                column: "DocumentsEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentTypes_DocumentsEntities_DocumentsEntityId",
                table: "DocumentTypes",
                column: "DocumentsEntityId",
                principalTable: "DocumentsEntities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Penalties_AbpTenants_TenantId",
                table: "Penalties",
                column: "TenantId",
                principalTable: "AbpTenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
