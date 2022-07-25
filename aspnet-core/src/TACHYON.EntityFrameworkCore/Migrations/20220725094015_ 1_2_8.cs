using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class _1_2_8 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Invoices_InvoiceNumber",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "ShippingRequestTripAccidentResolves");

            migrationBuilder.DropColumn(
                name: "DocumentContentType",
                table: "ShippingRequestTripAccidentResolves");

            migrationBuilder.DropColumn(
                name: "DocumentId",
                table: "ShippingRequestTripAccidentResolves");

            migrationBuilder.DropColumn(
                name: "DocumentName",
                table: "ShippingRequestTripAccidentResolves");


            migrationBuilder.AddColumn<bool>(
                name: "IsAppearAmount",
                table: "Vases",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "BulkUploadRef",
                table: "ShippingRequestTrips",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "CancelStatus",
                table: "ShippingRequestTrips",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<string>(
                name: "CanceledReason",
                table: "ShippingRequestTrips",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpectedDeliveryTime",
                table: "ShippingRequestTrips",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectedCancelingReason",
                table: "ShippingRequestTrips",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SupposedPickupDateFrom",
                table: "ShippingRequestTrips",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SupposedPickupDateTo",
                table: "ShippingRequestTrips",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Key",
                table: "ShippingRequestTripRejectReasons",
                maxLength: 60,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "ShippingRequestId",
                table: "ShippingRequestTripAccidents",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<bool>(
                name: "ApprovedByCarrier",
                table: "ShippingRequestTripAccidentResolves",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ApprovedByShipper",
                table: "ShippingRequestTripAccidentResolves",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "DriverId",
                table: "ShippingRequestTripAccidentResolves",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsApplied",
                table: "ShippingRequestTripAccidentResolves",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ResolveType",
                table: "ShippingRequestTripAccidentResolves",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "TruckId",
                table: "ShippingRequestTripAccidentResolves",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PricePackageOfferId",
                table: "ShippingRequestDirectRequests",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BulkUploadReference",
                table: "RoutPoints",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "InvoiceNumber",
                table: "Invoices",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NoteId",
                table: "DocumentFiles",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FinancialEmail",
                table: "AbpTenants",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FinancialName",
                table: "AbpTenants",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FinancialPhone",
                table: "AbpTenants",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EntityTemplates",
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
                    TemplateName = table.Column<string>(maxLength: 2, nullable: false),
                    SavedEntity = table.Column<string>(nullable: false),
                    SavedEntityId = table.Column<string>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    CreatorTenantId = table.Column<int>(nullable: true),
                    EntityType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FacilityWorkingHours",
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
                    FacilityId = table.Column<long>(nullable: false),
                    DayOfWeek = table.Column<int>(nullable: false),
                    StartTime = table.Column<TimeSpan>(nullable: false),
                    EndTime = table.Column<TimeSpan>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacilityWorkingHours", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacilityWorkingHours_Facilities_FacilityId",
                        column: x => x.FacilityId,
                        principalTable: "Facilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceNotes",
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
                    NoteType = table.Column<byte>(nullable: false),
                    Status = table.Column<byte>(nullable: false),
                    Remarks = table.Column<string>(nullable: true),
                    WaybillNumber = table.Column<string>(nullable: true),
                    VatAmount = table.Column<decimal>(nullable: false),
                    Price = table.Column<decimal>(nullable: false),
                    TotalValue = table.Column<decimal>(nullable: false),
                    ReferanceNumber = table.Column<string>(nullable: true),
                    InvoiceNumber = table.Column<long>(nullable: true),
                    VoidType = table.Column<int>(nullable: false),
                    IsManual = table.Column<bool>(nullable: false),
                    CanBePrinted = table.Column<bool>(nullable: false),
                    Note = table.Column<string>(nullable: true),
                    IsDrafted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoiceNotes_AbpTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                name: "Penalties",
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
                    PenaltyName = table.Column<string>(nullable: true),
                    PenaltyDescrption = table.Column<string>(nullable: true),
                    Type = table.Column<byte>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    SourceFeature = table.Column<string>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    DestinationTenantId = table.Column<int>(nullable: true),
                    ShippingRequestTripId = table.Column<int>(nullable: true),
                    PointId = table.Column<long>(nullable: true),
                    RoutPointFKId = table.Column<long>(nullable: true),
                    InvoiceId = table.Column<long>(nullable: true),
                    SubmitInvoiceId = table.Column<long>(nullable: true),
                    PenaltyComplaintId = table.Column<int>(nullable: true),
                    CommissionValue = table.Column<decimal>(nullable: false),
                    CommissionType = table.Column<byte>(nullable: false),
                    AmountPreCommestion = table.Column<decimal>(nullable: false),
                    AmountPostCommestion = table.Column<decimal>(nullable: false),
                    TaxVat = table.Column<decimal>(nullable: false),
                    VatAmount = table.Column<decimal>(nullable: false),
                    VatPostCommestion = table.Column<decimal>(nullable: false),
                    TotalAmount = table.Column<decimal>(nullable: false),
                    ItmePrice = table.Column<decimal>(nullable: false),
                    CommissionPercentageOrAddValue = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Penalties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Penalties_AbpTenants_DestinationTenantId",
                        column: x => x.DestinationTenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Penalties_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Penalties_RoutPoints_RoutPointFKId",
                        column: x => x.RoutPointFKId,
                        principalTable: "RoutPoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Penalties_ShippingRequestTrips_ShippingRequestTripId",
                        column: x => x.ShippingRequestTripId,
                        principalTable: "ShippingRequestTrips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Penalties_SubmitInvoices_SubmitInvoiceId",
                        column: x => x.SubmitInvoiceId,
                        principalTable: "SubmitInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Penalties_AbpTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShippingRequestAndTripNotes",
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
                    Note = table.Column<string>(nullable: true),
                    TripId = table.Column<int>(nullable: true),
                    ShippingRequetId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    Visibility = table.Column<byte>(nullable: false),
                    IsPrintedByWabillInvoice = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingRequestAndTripNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingRequestAndTripNotes_ShippingRequests_ShippingRequetId",
                        column: x => x.ShippingRequetId,
                        principalTable: "ShippingRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShippingRequestAndTripNotes_AbpTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShippingRequestAndTripNotes_ShippingRequestTrips_TripId",
                        column: x => x.TripId,
                        principalTable: "ShippingRequestTrips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShippingRequestPostPriceUpdates",
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
                    Action = table.Column<int>(nullable: false),
                    RejectionReason = table.Column<string>(nullable: true),
                    UpdateChanges = table.Column<string>(nullable: true),
                    IsApplied = table.Column<bool>(nullable: false),
                    PriceOfferId = table.Column<long>(nullable: true),
                    OfferStatus = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingRequestPostPriceUpdates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingRequestPostPriceUpdates_PriceOffers_PriceOfferId",
                        column: x => x.PriceOfferId,
                        principalTable: "PriceOffers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShippingRequestPostPriceUpdates_ShippingRequests_ShippingRequestId",
                        column: x => x.ShippingRequestId,
                        principalTable: "ShippingRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShippingRequestUpdates",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    ShippingRequestId = table.Column<long>(nullable: true),
                    EntityLogId = table.Column<Guid>(nullable: false),
                    PriceOfferId = table.Column<long>(nullable: false),
                    OldPriceOfferId = table.Column<long>(nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingRequestUpdates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingRequestUpdates_EntityHistoryLog_EntityLogId",
                        column: x => x.EntityLogId,
                        principalTable: "EntityHistoryLog",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShippingRequestUpdates_PriceOffers_OldPriceOfferId",
                        column: x => x.OldPriceOfferId,
                        principalTable: "PriceOffers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShippingRequestUpdates_PriceOffers_PriceOfferId",
                        column: x => x.PriceOfferId,
                        principalTable: "PriceOffers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShippingRequestUpdates_ShippingRequests_ShippingRequestId",
                        column: x => x.ShippingRequestId,
                        principalTable: "ShippingRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceNoteItems",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceNoteId = table.Column<long>(nullable: false),
                    TripId = table.Column<int>(nullable: true),
                    TripVasId = table.Column<long>(nullable: true),
                    Price = table.Column<decimal>(nullable: false),
                    VatAmount = table.Column<decimal>(nullable: false),
                    TotalAmount = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceNoteItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoiceNoteItems_InvoiceNotes_InvoiceNoteId",
                        column: x => x.InvoiceNoteId,
                        principalTable: "InvoiceNotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvoiceNoteItems_ShippingRequestTrips_TripId",
                        column: x => x.TripId,
                        principalTable: "ShippingRequestTrips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvoiceNoteItems_ShippingRequestTripVases_TripVasId",
                        column: x => x.TripVasId,
                        principalTable: "ShippingRequestTripVases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PricePackageOffers_TransportTypes_TransportTypeId",
                        column: x => x.TransportTypeId,
                        principalTable: "TransportTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PricePackageOffers_TrucksTypes_TrucksTypeId",
                        column: x => x.TrucksTypeId,
                        principalTable: "TrucksTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PenaltyComplaints",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PenaltyId = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    RejectReason = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PenaltyComplaints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PenaltyComplaints_Penalties_PenaltyId",
                        column: x => x.PenaltyId,
                        principalTable: "Penalties",
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
                name: "IX_Invoices_InvoiceNumber",
                table: "Invoices",
                column: "InvoiceNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentFiles_NoteId",
                table: "DocumentFiles",
                column: "NoteId");

            migrationBuilder.CreateIndex(
                name: "IX_FacilityWorkingHours_FacilityId",
                table: "FacilityWorkingHours",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceNoteItems_InvoiceNoteId",
                table: "InvoiceNoteItems",
                column: "InvoiceNoteId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceNoteItems_TripId",
                table: "InvoiceNoteItems",
                column: "TripId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceNoteItems_TripVasId",
                table: "InvoiceNoteItems",
                column: "TripVasId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceNotes_TenantId",
                table: "InvoiceNotes",
                column: "TenantId");

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
                name: "IX_Penalties_DestinationTenantId",
                table: "Penalties",
                column: "DestinationTenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Penalties_InvoiceId",
                table: "Penalties",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Penalties_RoutPointFKId",
                table: "Penalties",
                column: "RoutPointFKId");

            migrationBuilder.CreateIndex(
                name: "IX_Penalties_ShippingRequestTripId",
                table: "Penalties",
                column: "ShippingRequestTripId");

            migrationBuilder.CreateIndex(
                name: "IX_Penalties_SubmitInvoiceId",
                table: "Penalties",
                column: "SubmitInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Penalties_TenantId",
                table: "Penalties",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PenaltyComplaints_PenaltyId",
                table: "PenaltyComplaints",
                column: "PenaltyId",
                unique: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestAndTripNotes_ShippingRequetId",
                table: "ShippingRequestAndTripNotes",
                column: "ShippingRequetId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestAndTripNotes_TenantId",
                table: "ShippingRequestAndTripNotes",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestAndTripNotes_TripId",
                table: "ShippingRequestAndTripNotes",
                column: "TripId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestPostPriceUpdates_PriceOfferId",
                table: "ShippingRequestPostPriceUpdates",
                column: "PriceOfferId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestPostPriceUpdates_ShippingRequestId",
                table: "ShippingRequestPostPriceUpdates",
                column: "ShippingRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestUpdates_EntityLogId",
                table: "ShippingRequestUpdates",
                column: "EntityLogId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestUpdates_OldPriceOfferId",
                table: "ShippingRequestUpdates",
                column: "OldPriceOfferId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestUpdates_PriceOfferId",
                table: "ShippingRequestUpdates",
                column: "PriceOfferId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestUpdates_ShippingRequestId",
                table: "ShippingRequestUpdates",
                column: "ShippingRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentFiles_ShippingRequestAndTripNotes_NoteId",
                table: "DocumentFiles",
                column: "NoteId",
                principalTable: "ShippingRequestAndTripNotes",
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
                name: "FK_DocumentFiles_ShippingRequestAndTripNotes_NoteId",
                table: "DocumentFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestDirectRequests_PricePackageOffers_PricePackageOfferId",
                table: "ShippingRequestDirectRequests");

            migrationBuilder.DropTable(
                name: "EntityTemplates");

            migrationBuilder.DropTable(
                name: "FacilityWorkingHours");

            migrationBuilder.DropTable(
                name: "InvoiceNoteItems");

            migrationBuilder.DropTable(
                name: "PenaltyComplaints");

            migrationBuilder.DropTable(
                name: "PricePackageOfferItems");

            migrationBuilder.DropTable(
                name: "ShippingRequestAndTripNotes");

            migrationBuilder.DropTable(
                name: "ShippingRequestPostPriceUpdates");

            migrationBuilder.DropTable(
                name: "ShippingRequestUpdates");

            migrationBuilder.DropTable(
                name: "InvoiceNotes");

            migrationBuilder.DropTable(
                name: "Penalties");

            migrationBuilder.DropTable(
                name: "PricePackageOffers");

            migrationBuilder.DropTable(
                name: "NormalPricePackages");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequestDirectRequests_PricePackageOfferId",
                table: "ShippingRequestDirectRequests");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_InvoiceNumber",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_DocumentFiles_NoteId",
                table: "DocumentFiles");

            migrationBuilder.DropColumn(
                name: "IsAppearAmount",
                table: "Vases");

            migrationBuilder.DropColumn(
                name: "BulkUploadRef",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "CancelStatus",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "CanceledReason",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "ExpectedDeliveryTime",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "RejectedCancelingReason",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "SupposedPickupDateFrom",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "SupposedPickupDateTo",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "Key",
                table: "ShippingRequestTripRejectReasons");

            migrationBuilder.DropColumn(
                name: "ShippingRequestId",
                table: "ShippingRequestTripAccidents");

            migrationBuilder.DropColumn(
                name: "ApprovedByCarrier",
                table: "ShippingRequestTripAccidentResolves");

            migrationBuilder.DropColumn(
                name: "ApprovedByShipper",
                table: "ShippingRequestTripAccidentResolves");

            migrationBuilder.DropColumn(
                name: "DriverId",
                table: "ShippingRequestTripAccidentResolves");

            migrationBuilder.DropColumn(
                name: "IsApplied",
                table: "ShippingRequestTripAccidentResolves");

            migrationBuilder.DropColumn(
                name: "ResolveType",
                table: "ShippingRequestTripAccidentResolves");

            migrationBuilder.DropColumn(
                name: "TruckId",
                table: "ShippingRequestTripAccidentResolves");

            migrationBuilder.DropColumn(
                name: "PricePackageOfferId",
                table: "ShippingRequestDirectRequests");

            migrationBuilder.DropColumn(
                name: "BulkUploadReference",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "NoteId",
                table: "DocumentFiles");

            migrationBuilder.DropColumn(
                name: "FinancialEmail",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "FinancialName",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "FinancialPhone",
                table: "AbpTenants");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ShippingRequestTripAccidentResolves",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocumentContentType",
                table: "ShippingRequestTripAccidentResolves",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DocumentId",
                table: "ShippingRequestTripAccidentResolves",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocumentName",
                table: "ShippingRequestTripAccidentResolves",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "InvoiceNumber",
                table: "Invoices",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long));

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

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_InvoiceNumber",
                table: "Invoices",
                column: "InvoiceNumber",
                unique: true,
                filter: "[InvoiceNumber] IS NOT NULL");
        }
    }
}
