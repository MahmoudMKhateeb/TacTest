using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class _3810 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PricePackageOffers_Cities_DestinationCityId",
                table: "PricePackageOffers");

            migrationBuilder.DropForeignKey(
                name: "FK_PricePackageOffers_NormalPricePackages_NormalPricePackageId",
                table: "PricePackageOffers");

            migrationBuilder.DropForeignKey(
                name: "FK_PricePackageOffers_Cities_OriginCityId",
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

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_ShippingTypes_ShippingTypeId",
                table: "ShippingRequests");

            migrationBuilder.DropTable(
                name: "PricePackageOfferItems");

            migrationBuilder.DropTable(
                name: "TmsPriceOfferPackageOffers");

            migrationBuilder.DropTable(
                name: "NormalPricePackages");

            migrationBuilder.DropTable(
                name: "TmsPricePackages");

            //migrationBuilder.DropIndex(
            //    name: "IX_ShippingRequests_ShippingTypeId",
            //    table: "ShippingRequests");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequestDirectRequests_PricePackageOfferId",
                table: "ShippingRequestDirectRequests");

            migrationBuilder.DropIndex(
                name: "IX_PricePackageOffers_DestinationCityId",
                table: "PricePackageOffers");

            migrationBuilder.DropIndex(
                name: "IX_PricePackageOffers_NormalPricePackageId",
                table: "PricePackageOffers");

            migrationBuilder.DropIndex(
                name: "IX_PricePackageOffers_OriginCityId",
                table: "PricePackageOffers");

            migrationBuilder.DropIndex(
                name: "IX_PricePackageOffers_TenantId",
                table: "PricePackageOffers");

            migrationBuilder.DropIndex(
                name: "IX_PricePackageOffers_TransportTypeId",
                table: "PricePackageOffers");

            migrationBuilder.DropIndex(
                name: "IX_PricePackageOffers_TrucksTypeId",
                table: "PricePackageOffers");

            migrationBuilder.DropColumn(
                name: "PricePackageOfferId",
                table: "ShippingRequestDirectRequests");

            migrationBuilder.DropColumn(
                name: "CommissionAmount",
                table: "PricePackageOffers");

            migrationBuilder.DropColumn(
                name: "CommissionPercentageOrAddValue",
                table: "PricePackageOffers");

            migrationBuilder.DropColumn(
                name: "CommissionPercentageOrAddValueForMultipleDrop",
                table: "PricePackageOffers");

            migrationBuilder.DropColumn(
                name: "CommissionPercentageOrAddValueForSingleDrop",
                table: "PricePackageOffers");

            migrationBuilder.DropColumn(
                name: "CommissionType",
                table: "PricePackageOffers");

            migrationBuilder.DropColumn(
                name: "DestinationCityId",
                table: "PricePackageOffers");

            migrationBuilder.DropColumn(
                name: "DetailsTotalCommission",
                table: "PricePackageOffers");

            migrationBuilder.DropColumn(
                name: "DetailsTotalPricePostCommissionPreVat",
                table: "PricePackageOffers");

            migrationBuilder.DropColumn(
                name: "DetailsTotalPricePreCommissionPreVat",
                table: "PricePackageOffers");

            migrationBuilder.DropColumn(
                name: "DetailsTotalVatAmountPreCommission",
                table: "PricePackageOffers");

            migrationBuilder.DropColumn(
                name: "DetailsTotalVatPostCommission",
                table: "PricePackageOffers");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "PricePackageOffers");

            migrationBuilder.DropColumn(
                name: "ItemCommissionAmount",
                table: "PricePackageOffers");

            migrationBuilder.DropColumn(
                name: "ItemPrice",
                table: "PricePackageOffers");

            migrationBuilder.DropColumn(
                name: "ItemSubTotalAmountWithCommission",
                table: "PricePackageOffers");

            migrationBuilder.DropColumn(
                name: "ItemTotalAmount",
                table: "PricePackageOffers");

            migrationBuilder.DropColumn(
                name: "ItemTotalAmountWithCommission",
                table: "PricePackageOffers");

            migrationBuilder.DropColumn(
                name: "ItemVatAmount",
                table: "PricePackageOffers");

            migrationBuilder.DropColumn(
                name: "ItemVatAmountWithCommission",
                table: "PricePackageOffers");

            migrationBuilder.DropColumn(
                name: "ItemsTotalCommission",
                table: "PricePackageOffers");

            migrationBuilder.DropColumn(
                name: "ItemsTotalPricePostCommissionPreVat",
                table: "PricePackageOffers");

            migrationBuilder.DropColumn(
                name: "ItemsTotalPricePreCommissionPreVat",
                table: "PricePackageOffers");

            migrationBuilder.DropColumn(
                name: "ItemsTotalVatAmountPreCommission",
                table: "PricePackageOffers");

            migrationBuilder.DropColumn(
                name: "ItemsTotalVatPostCommission",
                table: "PricePackageOffers");

            migrationBuilder.DropColumn(
                name: "NormalPricePackageId",
                table: "PricePackageOffers");

            migrationBuilder.DropColumn(
                name: "OriginCityId",
                table: "PricePackageOffers");

            migrationBuilder.DropColumn(
                name: "PriceType",
                table: "PricePackageOffers");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "PricePackageOffers");

            migrationBuilder.DropColumn(
                name: "SourceId",
                table: "PricePackageOffers");

            migrationBuilder.DropColumn(
                name: "SubTotalAmount",
                table: "PricePackageOffers");

            migrationBuilder.DropColumn(
                name: "SubTotalAmountWithCommission",
                table: "PricePackageOffers");

            migrationBuilder.DropColumn(
                name: "TaxVat",
                table: "PricePackageOffers");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "PricePackageOffers");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "PricePackageOffers");

            migrationBuilder.DropColumn(
                name: "TotalAmountWithCommission",
                table: "PricePackageOffers");

            migrationBuilder.DropColumn(
                name: "TransportTypeId",
                table: "PricePackageOffers");

            migrationBuilder.DropColumn(
                name: "TrucksTypeId",
                table: "PricePackageOffers");

            migrationBuilder.DropColumn(
                name: "VatAmount",
                table: "PricePackageOffers");

            migrationBuilder.DropColumn(
                name: "VatAmountWithCommission",
                table: "PricePackageOffers");

            migrationBuilder.AddColumn<long>(
                name: "RoutePointId",
                table: "ShippingRequestTripVases",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SaasInvoicingActivation",
                table: "ShippingRequestTrips",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "OriginFacilityId",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "RoundTripType",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PricePackageId",
                table: "ServiceAreas",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AdditionalStepWorkFlowVersion",
                table: "RoutPoints",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AppointmentDateTime",
                table: "RoutPoints",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AppointmentNumber",
                table: "RoutPoints",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasAppointmentVas",
                table: "RoutPoints",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasClearanceVas",
                table: "RoutPoints",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NeedsAppointment",
                table: "RoutPoints",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NeedsClearance",
                table: "RoutPoints",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PointOrder",
                table: "RoutPoints",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "PricePackageId",
                table: "PricePackageOffers",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DirectRequestId",
                table: "PricePackageOffers",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PriceOfferId",
                table: "PricePackageOffers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Flag",
                table: "GoodCategories",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FacilityType",
                table: "Facilities",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "AdditionalStepTransitions",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    RoutePointId = table.Column<long>(nullable: false),
                    AdditionalStepType = table.Column<int>(nullable: false),
                    RoutePointDocumentType = table.Column<byte>(nullable: true),
                    IsFile = table.Column<bool>(nullable: false),
                    IsReset = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdditionalStepTransitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdditionalStepTransitions_RoutPoints_RoutePointId",
                        column: x => x.RoutePointId,
                        principalTable: "RoutPoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PricePackages",
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
                    PricePackageReference = table.Column<string>(nullable: true),
                    DisplayName = table.Column<string>(maxLength: 100, nullable: false),
                    TenantId = table.Column<int>(nullable: false),
                    TransportTypeId = table.Column<int>(nullable: false),
                    TruckTypeId = table.Column<long>(nullable: false),
                    OriginCityId = table.Column<int>(nullable: true),
                    DestinationCityId = table.Column<int>(nullable: true),
                    DestinationTenantId = table.Column<int>(nullable: true),
                    RouteType = table.Column<byte>(nullable: false),
                    ShippingTypeId = table.Column<int>(nullable: true),
                    TotalPrice = table.Column<decimal>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    ProposalId = table.Column<int>(nullable: true),
                    AppendixId = table.Column<int>(nullable: true),
                    UsageType = table.Column<int>(nullable: false),
                    PricePerAdditionalDrop = table.Column<decimal>(nullable: true),
                    DirectRequestPrice = table.Column<decimal>(nullable: true),
                    OriginCountryId = table.Column<int>(nullable: true),
                    ProjectName = table.Column<string>(nullable: true),
                    ScopeOfWork = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PricePackages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PricePackages_PricePackageAppendixes_AppendixId",
                        column: x => x.AppendixId,
                        principalTable: "PricePackageAppendixes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PricePackages_Cities_DestinationCityId",
                        column: x => x.DestinationCityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PricePackages_AbpTenants_DestinationTenantId",
                        column: x => x.DestinationTenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PricePackages_Cities_OriginCityId",
                        column: x => x.OriginCityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PricePackages_PricePackageProposals_ProposalId",
                        column: x => x.ProposalId,
                        principalTable: "PricePackageProposals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PricePackages_ShippingTypes_ShippingTypeId",
                        column: x => x.ShippingTypeId,
                        principalTable: "ShippingTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PricePackages_AbpTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PricePackages_TransportTypes_TransportTypeId",
                        column: x => x.TransportTypeId,
                        principalTable: "TransportTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PricePackages_TrucksTypes_TruckTypeId",
                        column: x => x.TruckTypeId,
                        principalTable: "TrucksTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTripVases_RoutePointId",
                table: "ShippingRequestTripVases",
                column: "RoutePointId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequests_OriginFacilityId",
                table: "ShippingRequests",
                column: "OriginFacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceAreas_PricePackageId",
                table: "ServiceAreas",
                column: "PricePackageId");

            migrationBuilder.CreateIndex(
                name: "IX_PricePackageOffers_DirectRequestId",
                table: "PricePackageOffers",
                column: "DirectRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_PricePackageOffers_PriceOfferId",
                table: "PricePackageOffers",
                column: "PriceOfferId");

            migrationBuilder.CreateIndex(
                name: "IX_PricePackageOffers_PricePackageId",
                table: "PricePackageOffers",
                column: "PricePackageId");

            migrationBuilder.CreateIndex(
                name: "IX_AdditionalStepTransitions_RoutePointId",
                table: "AdditionalStepTransitions",
                column: "RoutePointId");

            migrationBuilder.CreateIndex(
                name: "IX_PricePackages_AppendixId",
                table: "PricePackages",
                column: "AppendixId");

            migrationBuilder.CreateIndex(
                name: "IX_PricePackages_DestinationCityId",
                table: "PricePackages",
                column: "DestinationCityId");

            migrationBuilder.CreateIndex(
                name: "IX_PricePackages_DestinationTenantId",
                table: "PricePackages",
                column: "DestinationTenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PricePackages_OriginCityId",
                table: "PricePackages",
                column: "OriginCityId");

            migrationBuilder.CreateIndex(
                name: "IX_PricePackages_ProposalId",
                table: "PricePackages",
                column: "ProposalId");

            migrationBuilder.CreateIndex(
                name: "IX_PricePackages_ShippingTypeId",
                table: "PricePackages",
                column: "ShippingTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PricePackages_TenantId",
                table: "PricePackages",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PricePackages_TransportTypeId",
                table: "PricePackages",
                column: "TransportTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PricePackages_TruckTypeId",
                table: "PricePackages",
                column: "TruckTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_PricePackageOffers_ShippingRequestDirectRequests_DirectRequestId",
                table: "PricePackageOffers",
                column: "DirectRequestId",
                principalTable: "ShippingRequestDirectRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PricePackageOffers_PriceOffers_PriceOfferId",
                table: "PricePackageOffers",
                column: "PriceOfferId",
                principalTable: "PriceOffers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PricePackageOffers_PricePackages_PricePackageId",
                table: "PricePackageOffers",
                column: "PricePackageId",
                principalTable: "PricePackages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceAreas_PricePackages_PricePackageId",
                table: "ServiceAreas",
                column: "PricePackageId",
                principalTable: "PricePackages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequests_Facilities_OriginFacilityId",
                table: "ShippingRequests",
                column: "OriginFacilityId",
                principalTable: "Facilities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequestTripVases_RoutPoints_RoutePointId",
                table: "ShippingRequestTripVases",
                column: "RoutePointId",
                principalTable: "RoutPoints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PricePackageOffers_ShippingRequestDirectRequests_DirectRequestId",
                table: "PricePackageOffers");

            migrationBuilder.DropForeignKey(
                name: "FK_PricePackageOffers_PriceOffers_PriceOfferId",
                table: "PricePackageOffers");

            migrationBuilder.DropForeignKey(
                name: "FK_PricePackageOffers_PricePackages_PricePackageId",
                table: "PricePackageOffers");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceAreas_PricePackages_PricePackageId",
                table: "ServiceAreas");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_Facilities_OriginFacilityId",
                table: "ShippingRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestTripVases_RoutPoints_RoutePointId",
                table: "ShippingRequestTripVases");

            migrationBuilder.DropTable(
                name: "AdditionalStepTransitions");

            migrationBuilder.DropTable(
                name: "PricePackages");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequestTripVases_RoutePointId",
                table: "ShippingRequestTripVases");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequests_OriginFacilityId",
                table: "ShippingRequests");

            migrationBuilder.DropIndex(
                name: "IX_ServiceAreas_PricePackageId",
                table: "ServiceAreas");

            migrationBuilder.DropIndex(
                name: "IX_PricePackageOffers_DirectRequestId",
                table: "PricePackageOffers");

            migrationBuilder.DropIndex(
                name: "IX_PricePackageOffers_PriceOfferId",
                table: "PricePackageOffers");

            migrationBuilder.DropIndex(
                name: "IX_PricePackageOffers_PricePackageId",
                table: "PricePackageOffers");

            migrationBuilder.DropColumn(
                name: "RoutePointId",
                table: "ShippingRequestTripVases");

            migrationBuilder.DropColumn(
                name: "SaasInvoicingActivation",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "OriginFacilityId",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "RoundTripType",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "PricePackageId",
                table: "ServiceAreas");

            migrationBuilder.DropColumn(
                name: "AdditionalStepWorkFlowVersion",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "AppointmentDateTime",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "AppointmentNumber",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "HasAppointmentVas",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "HasClearanceVas",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "NeedsAppointment",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "NeedsClearance",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "PointOrder",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "DirectRequestId",
                table: "PricePackageOffers");

            migrationBuilder.DropColumn(
                name: "PriceOfferId",
                table: "PricePackageOffers");

            migrationBuilder.DropColumn(
                name: "Flag",
                table: "GoodCategories");

            migrationBuilder.DropColumn(
                name: "FacilityType",
                table: "Facilities");

            migrationBuilder.AddColumn<long>(
                name: "PricePackageOfferId",
                table: "ShippingRequestDirectRequests",
                type: "bigint",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PricePackageId",
                table: "PricePackageOffers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CommissionAmount",
                table: "PricePackageOffers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CommissionPercentageOrAddValue",
                table: "PricePackageOffers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CommissionPercentageOrAddValueForMultipleDrop",
                table: "PricePackageOffers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CommissionPercentageOrAddValueForSingleDrop",
                table: "PricePackageOffers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<byte>(
                name: "CommissionType",
                table: "PricePackageOffers",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<int>(
                name: "DestinationCityId",
                table: "PricePackageOffers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DetailsTotalCommission",
                table: "PricePackageOffers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DetailsTotalPricePostCommissionPreVat",
                table: "PricePackageOffers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DetailsTotalPricePreCommissionPreVat",
                table: "PricePackageOffers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DetailsTotalVatAmountPreCommission",
                table: "PricePackageOffers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DetailsTotalVatPostCommission",
                table: "PricePackageOffers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "PricePackageOffers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "ItemCommissionAmount",
                table: "PricePackageOffers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ItemPrice",
                table: "PricePackageOffers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ItemSubTotalAmountWithCommission",
                table: "PricePackageOffers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ItemTotalAmount",
                table: "PricePackageOffers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ItemTotalAmountWithCommission",
                table: "PricePackageOffers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ItemVatAmount",
                table: "PricePackageOffers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ItemVatAmountWithCommission",
                table: "PricePackageOffers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ItemsTotalCommission",
                table: "PricePackageOffers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ItemsTotalPricePostCommissionPreVat",
                table: "PricePackageOffers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ItemsTotalPricePreCommissionPreVat",
                table: "PricePackageOffers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ItemsTotalVatAmountPreCommission",
                table: "PricePackageOffers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ItemsTotalVatPostCommission",
                table: "PricePackageOffers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "NormalPricePackageId",
                table: "PricePackageOffers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OriginCityId",
                table: "PricePackageOffers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "PriceType",
                table: "PricePackageOffers",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "PricePackageOffers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "SourceId",
                table: "PricePackageOffers",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotalAmount",
                table: "PricePackageOffers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotalAmountWithCommission",
                table: "PricePackageOffers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxVat",
                table: "PricePackageOffers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "PricePackageOffers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                table: "PricePackageOffers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmountWithCommission",
                table: "PricePackageOffers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "TransportTypeId",
                table: "PricePackageOffers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "TrucksTypeId",
                table: "PricePackageOffers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<decimal>(
                name: "VatAmount",
                table: "PricePackageOffers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "VatAmountWithCommission",
                table: "PricePackageOffers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "NormalPricePackages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppendixId = table.Column<int>(type: "int", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DestinationCityId = table.Column<int>(type: "int", nullable: true),
                    DirectRequestPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsMultiDrop = table.Column<bool>(type: "bit", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    MarcketPlaceRequestPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OriginCityId = table.Column<int>(type: "int", nullable: true),
                    PricePackageId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PricePerExtraDrop = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TachyonMSRequestPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    TransportTypeId = table.Column<int>(type: "int", nullable: false),
                    TrucksTypeId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NormalPricePackages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NormalPricePackages_PricePackageAppendixes_AppendixId",
                        column: x => x.AppendixId,
                        principalTable: "PricePackageAppendixes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                name: "PricePackageOfferItems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BidNormalPricePackageId = table.Column<long>(type: "bigint", nullable: false),
                    CommissionAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CommissionPercentageOrAddValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CommissionPercentageOrAddValueForMultipleDrop = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CommissionPercentageOrAddValueForSingleDrop = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
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
                    table.PrimaryKey("PK_PricePackageOfferItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PricePackageOfferItems_PricePackageOffers_BidNormalPricePackageId",
                        column: x => x.BidNormalPricePackageId,
                        principalTable: "PricePackageOffers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TmsPricePackages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppendixId = table.Column<int>(type: "int", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DestinationCityId = table.Column<int>(type: "int", nullable: true),
                    DestinationTenantId = table.Column<int>(type: "int", nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    OriginCityId = table.Column<int>(type: "int", nullable: true),
                    PricePackageId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProposalId = table.Column<int>(type: "int", nullable: true),
                    RouteType = table.Column<byte>(type: "tinyint", nullable: false),
                    ShippingTypeId = table.Column<int>(type: "int", nullable: true),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TransportTypeId = table.Column<int>(type: "int", nullable: false),
                    TrucksTypeId = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TmsPricePackages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TmsPricePackages_PricePackageAppendixes_AppendixId",
                        column: x => x.AppendixId,
                        principalTable: "PricePackageAppendixes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TmsPricePackages_Cities_DestinationCityId",
                        column: x => x.DestinationCityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TmsPricePackages_AbpTenants_DestinationTenantId",
                        column: x => x.DestinationTenantId,
                        principalTable: "AbpTenants",
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
                        name: "FK_TmsPricePackages_ShippingTypes_ShippingTypeId",
                        column: x => x.ShippingTypeId,
                        principalTable: "ShippingTypes",
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

            migrationBuilder.CreateTable(
                name: "TmsPriceOfferPackageOffers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DirectRequestId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    NormalPricePackageId = table.Column<int>(type: "int", nullable: true),
                    PriceOfferId = table.Column<long>(type: "bigint", nullable: true),
                    TmsPricePackageId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TmsPriceOfferPackageOffers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TmsPriceOfferPackageOffers_ShippingRequestDirectRequests_DirectRequestId",
                        column: x => x.DirectRequestId,
                        principalTable: "ShippingRequestDirectRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TmsPriceOfferPackageOffers_NormalPricePackages_NormalPricePackageId",
                        column: x => x.NormalPricePackageId,
                        principalTable: "NormalPricePackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TmsPriceOfferPackageOffers_PriceOffers_PriceOfferId",
                        column: x => x.PriceOfferId,
                        principalTable: "PriceOffers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TmsPriceOfferPackageOffers_TmsPricePackages_TmsPricePackageId",
                        column: x => x.TmsPricePackageId,
                        principalTable: "TmsPricePackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequests_ShippingTypeId",
                table: "ShippingRequests",
                column: "ShippingTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestDirectRequests_PricePackageOfferId",
                table: "ShippingRequestDirectRequests",
                column: "PricePackageOfferId");

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
                name: "IX_NormalPricePackages_AppendixId",
                table: "NormalPricePackages",
                column: "AppendixId");

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
                name: "IX_TmsPriceOfferPackageOffers_DirectRequestId",
                table: "TmsPriceOfferPackageOffers",
                column: "DirectRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_TmsPriceOfferPackageOffers_NormalPricePackageId",
                table: "TmsPriceOfferPackageOffers",
                column: "NormalPricePackageId");

            migrationBuilder.CreateIndex(
                name: "IX_TmsPriceOfferPackageOffers_PriceOfferId",
                table: "TmsPriceOfferPackageOffers",
                column: "PriceOfferId");

            migrationBuilder.CreateIndex(
                name: "IX_TmsPriceOfferPackageOffers_TmsPricePackageId",
                table: "TmsPriceOfferPackageOffers",
                column: "TmsPricePackageId");

            migrationBuilder.CreateIndex(
                name: "IX_TmsPricePackages_AppendixId",
                table: "TmsPricePackages",
                column: "AppendixId");

            migrationBuilder.CreateIndex(
                name: "IX_TmsPricePackages_DestinationCityId",
                table: "TmsPricePackages",
                column: "DestinationCityId");

            migrationBuilder.CreateIndex(
                name: "IX_TmsPricePackages_DestinationTenantId",
                table: "TmsPricePackages",
                column: "DestinationTenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TmsPricePackages_OriginCityId",
                table: "TmsPricePackages",
                column: "OriginCityId");

            migrationBuilder.CreateIndex(
                name: "IX_TmsPricePackages_ProposalId",
                table: "TmsPricePackages",
                column: "ProposalId");

            migrationBuilder.CreateIndex(
                name: "IX_TmsPricePackages_ShippingTypeId",
                table: "TmsPricePackages",
                column: "ShippingTypeId");

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
                name: "FK_PricePackageOffers_Cities_DestinationCityId",
                table: "PricePackageOffers",
                column: "DestinationCityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PricePackageOffers_NormalPricePackages_NormalPricePackageId",
                table: "PricePackageOffers",
                column: "NormalPricePackageId",
                principalTable: "NormalPricePackages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PricePackageOffers_Cities_OriginCityId",
                table: "PricePackageOffers",
                column: "OriginCityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PricePackageOffers_AbpTenants_TenantId",
                table: "PricePackageOffers",
                column: "TenantId",
                principalTable: "AbpTenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PricePackageOffers_TransportTypes_TransportTypeId",
                table: "PricePackageOffers",
                column: "TransportTypeId",
                principalTable: "TransportTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PricePackageOffers_TrucksTypes_TrucksTypeId",
                table: "PricePackageOffers",
                column: "TrucksTypeId",
                principalTable: "TrucksTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequestDirectRequests_PricePackageOffers_PricePackageOfferId",
                table: "ShippingRequestDirectRequests",
                column: "PricePackageOfferId",
                principalTable: "PricePackageOffers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequests_ShippingTypes_ShippingTypeId",
                table: "ShippingRequests",
                column: "ShippingTypeId",
                principalTable: "ShippingTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
