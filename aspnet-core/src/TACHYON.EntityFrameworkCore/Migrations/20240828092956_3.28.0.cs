using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class _3280 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EquipNumber",
                table: "Trucks",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ContainerReturnDate",
                table: "ShippingRequestTrips",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Distance",
                table: "ShippingRequestTrips",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DriverCommission",
                table: "ShippingRequestTrips",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DriverWorkingHour",
                table: "ShippingRequestTrips",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsContainerReturned",
                table: "ShippingRequestTrips",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LoadingType",
                table: "ShippingRequestTrips",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "ShippingRequestTrips",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReplacedDriverCommission",
                table: "ShippingRequestTrips",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReplacedDriverDistance",
                table: "ShippingRequestTrips",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReplacedDriverWorkingHour",
                table: "ShippingRequestTrips",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "ReplacedTruckId",
                table: "ShippingRequestTrips",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ReplacesDriverId",
                table: "ShippingRequestTrips",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SabOrderId",
                table: "ShippingRequestTrips",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SalesOfficeType",
                table: "ShippingRequestTrips",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ShipperInvoiceNo",
                table: "ShippingRequestTrips",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShipperReference",
                table: "ShippingRequestTrips",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Distance",
                table: "RoutPoints",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "DriverUserId",
                table: "RoutPoints",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DriverWorkingHour",
                table: "RoutPoints",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StorageDays",
                table: "RoutPoints",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "StoragePricePerDay",
                table: "RoutPoints",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TruckId",
                table: "RoutPoints",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MoiNumber",
                table: "Actors",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "MobileNumber",
                table: "Actors",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "InvoiceDueDays",
                table: "Actors",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Actors",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Actors",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "AccountType",
                table: "Actors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BuildingCode",
                table: "Actors",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CityId",
                table: "Actors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerGroup",
                table: "Actors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "District",
                table: "Actors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Division",
                table: "Actors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Actors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Actors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "Actors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Reconsaccoun",
                table: "Actors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Region",
                table: "Actors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SalesGroup",
                table: "Actors",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SalesOfficeType",
                table: "Actors",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TrasportationZone",
                table: "Actors",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "actorDischannelEnum",
                table: "Actors",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSAB",
                table: "AbpTenants",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "SaasPricePackages",
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
                    DisplayName = table.Column<string>(nullable: false),
                    TenantId = table.Column<int>(nullable: false),
                    TransportTypeId = table.Column<int>(nullable: false),
                    TruckTypeId = table.Column<long>(nullable: false),
                    OriginCityId = table.Column<int>(nullable: true),
                    DestinationCityId = table.Column<int>(nullable: true),
                    ActorShipperId = table.Column<int>(nullable: true),
                    ActorShipperPrice = table.Column<decimal>(nullable: false),
                    ShippingTypeId = table.Column<int>(nullable: true),
                    GoodCategoryId = table.Column<int>(nullable: true),
                    tripLoadingType = table.Column<int>(nullable: true),
                    RoundTripType = table.Column<byte>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaasPricePackages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaasPricePackages_Actors_ActorShipperId",
                        column: x => x.ActorShipperId,
                        principalTable: "Actors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaasPricePackages_Cities_DestinationCityId",
                        column: x => x.DestinationCityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaasPricePackages_GoodCategories_GoodCategoryId",
                        column: x => x.GoodCategoryId,
                        principalTable: "GoodCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaasPricePackages_Cities_OriginCityId",
                        column: x => x.OriginCityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaasPricePackages_TransportTypes_TransportTypeId",
                        column: x => x.TransportTypeId,
                        principalTable: "TransportTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SaasPricePackages_TrucksTypes_TruckTypeId",
                        column: x => x.TruckTypeId,
                        principalTable: "TrucksTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTrips_ReplacedTruckId",
                table: "ShippingRequestTrips",
                column: "ReplacedTruckId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTrips_ReplacesDriverId",
                table: "ShippingRequestTrips",
                column: "ReplacesDriverId");

            migrationBuilder.CreateIndex(
                name: "IX_Actors_CityId",
                table: "Actors",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_SaasPricePackages_ActorShipperId",
                table: "SaasPricePackages",
                column: "ActorShipperId");

            migrationBuilder.CreateIndex(
                name: "IX_SaasPricePackages_DestinationCityId",
                table: "SaasPricePackages",
                column: "DestinationCityId");

            migrationBuilder.CreateIndex(
                name: "IX_SaasPricePackages_GoodCategoryId",
                table: "SaasPricePackages",
                column: "GoodCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_SaasPricePackages_OriginCityId",
                table: "SaasPricePackages",
                column: "OriginCityId");

            migrationBuilder.CreateIndex(
                name: "IX_SaasPricePackages_TransportTypeId",
                table: "SaasPricePackages",
                column: "TransportTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SaasPricePackages_TruckTypeId",
                table: "SaasPricePackages",
                column: "TruckTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Actors_Cities_CityId",
                table: "Actors",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequestTrips_Trucks_ReplacedTruckId",
                table: "ShippingRequestTrips",
                column: "ReplacedTruckId",
                principalTable: "Trucks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequestTrips_AbpUsers_ReplacesDriverId",
                table: "ShippingRequestTrips",
                column: "ReplacesDriverId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Actors_Cities_CityId",
                table: "Actors");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestTrips_Trucks_ReplacedTruckId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestTrips_AbpUsers_ReplacesDriverId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropTable(
                name: "SaasPricePackages");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequestTrips_ReplacedTruckId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequestTrips_ReplacesDriverId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropIndex(
                name: "IX_Actors_CityId",
                table: "Actors");

            migrationBuilder.DropColumn(
                name: "EquipNumber",
                table: "Trucks");

            migrationBuilder.DropColumn(
                name: "ContainerReturnDate",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "Distance",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "DriverCommission",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "DriverWorkingHour",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "IsContainerReturned",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "LoadingType",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "ReplacedDriverCommission",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "ReplacedDriverDistance",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "ReplacedDriverWorkingHour",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "ReplacedTruckId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "ReplacesDriverId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "SabOrderId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "SalesOfficeType",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "ShipperInvoiceNo",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "ShipperReference",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "Distance",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "DriverUserId",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "DriverWorkingHour",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "StorageDays",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "StoragePricePerDay",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "TruckId",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "AccountType",
                table: "Actors");

            migrationBuilder.DropColumn(
                name: "BuildingCode",
                table: "Actors");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "Actors");

            migrationBuilder.DropColumn(
                name: "CustomerGroup",
                table: "Actors");

            migrationBuilder.DropColumn(
                name: "District",
                table: "Actors");

            migrationBuilder.DropColumn(
                name: "Division",
                table: "Actors");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Actors");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Actors");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "Actors");

            migrationBuilder.DropColumn(
                name: "Reconsaccoun",
                table: "Actors");

            migrationBuilder.DropColumn(
                name: "Region",
                table: "Actors");

            migrationBuilder.DropColumn(
                name: "SalesGroup",
                table: "Actors");

            migrationBuilder.DropColumn(
                name: "SalesOfficeType",
                table: "Actors");

            migrationBuilder.DropColumn(
                name: "TrasportationZone",
                table: "Actors");

            migrationBuilder.DropColumn(
                name: "actorDischannelEnum",
                table: "Actors");

            migrationBuilder.DropColumn(
                name: "IsSAB",
                table: "AbpTenants");

            migrationBuilder.AlterColumn<string>(
                name: "MoiNumber",
                table: "Actors",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MobileNumber",
                table: "Actors",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "InvoiceDueDays",
                table: "Actors",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Actors",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Actors",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
