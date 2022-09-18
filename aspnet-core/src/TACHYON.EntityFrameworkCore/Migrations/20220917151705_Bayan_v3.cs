using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class Bayan_v3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BayanIntegrationId",
                table: "UnitOfMeasures",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BayanIntegrationId",
                table: "Cities",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RegionId",
                table: "Cities",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DriverIssueNumber",
                table: "AbpUsers",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CountryId",
                table: "AbpTenants",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CityId",
                table: "AbpTenants",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "BayanIntegrationResults",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    ActionName = table.Column<string>(nullable: false),
                    InputJson = table.Column<string>(nullable: false),
                    ResponseJson = table.Column<string>(nullable: true),
                    Version = table.Column<string>(nullable: true),
                    ShippingRequestTripId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BayanIntegrationResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BayanIntegrationResults_ShippingRequestTrips_ShippingRequestTripId",
                        column: x => x.ShippingRequestTripId,
                        principalTable: "ShippingRequestTrips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Regions",
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
                    Name = table.Column<string>(nullable: false),
                    BayanIntegrationId = table.Column<int>(nullable: false),
                    CountyId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Regions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Regions_Counties_CountyId",
                        column: x => x.CountyId,
                        principalTable: "Counties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cities_RegionId",
                table: "Cities",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_CityId",
                table: "AbpTenants",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_CountryId",
                table: "AbpTenants",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_BayanIntegrationResults_ShippingRequestTripId",
                table: "BayanIntegrationResults",
                column: "ShippingRequestTripId");

            migrationBuilder.CreateIndex(
                name: "IX_Regions_CountyId",
                table: "Regions",
                column: "CountyId");

            migrationBuilder.AddForeignKey(
                name: "FK_AbpTenants_Cities_CityId",
                table: "AbpTenants",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AbpTenants_Counties_CountryId",
                table: "AbpTenants",
                column: "CountryId",
                principalTable: "Counties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Cities_Regions_RegionId",
                table: "Cities",
                column: "RegionId",
                principalTable: "Regions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AbpTenants_Cities_CityId",
                table: "AbpTenants");

            migrationBuilder.DropForeignKey(
                name: "FK_AbpTenants_Counties_CountryId",
                table: "AbpTenants");

            migrationBuilder.DropForeignKey(
                name: "FK_Cities_Regions_RegionId",
                table: "Cities");

            migrationBuilder.DropTable(
                name: "BayanIntegrationResults");

            migrationBuilder.DropTable(
                name: "Regions");

            migrationBuilder.DropIndex(
                name: "IX_Cities_RegionId",
                table: "Cities");

            migrationBuilder.DropIndex(
                name: "IX_AbpTenants_CityId",
                table: "AbpTenants");

            migrationBuilder.DropIndex(
                name: "IX_AbpTenants_CountryId",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "BayanIntegrationId",
                table: "UnitOfMeasures");

            migrationBuilder.DropColumn(
                name: "BayanIntegrationId",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "RegionId",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "DriverIssueNumber",
                table: "AbpUsers");

            migrationBuilder.AlterColumn<int>(
                name: "CountryId",
                table: "AbpTenants",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CityId",
                table: "AbpTenants",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
