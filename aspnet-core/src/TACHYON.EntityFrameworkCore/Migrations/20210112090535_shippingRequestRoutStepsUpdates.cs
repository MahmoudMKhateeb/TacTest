using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class shippingRequestRoutStepsUpdates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoutSteps_Trailers_AssignedTrailerId",
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
                name: "Name",
                table: "GoodsDetails");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "GoodsDetails");

            migrationBuilder.AlterColumn<long>(
                name: "ShippingRequestId",
                table: "RoutSteps",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "AssignedTrailerId",
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

            migrationBuilder.AddColumn<int>(
                name: "NumberOfPackingType",
                table: "GoodsDetails",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PackingType",
                table: "GoodsDetails",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ShippingRequestId",
                table: "GoodsDetails",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "TotalAmount",
                table: "GoodsDetails",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FatherId",
                table: "GoodCategories",
                nullable: true);

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
                    TenantId = table.Column<int>(nullable: false),
                    CityId = table.Column<int>(nullable: false),
                    PickingTypeId = table.Column<int>(nullable: true),
                    Latitude = table.Column<string>(maxLength: 256, nullable: false),
                    Longitude = table.Column<string>(maxLength: 256, nullable: false),
                    FacilityId = table.Column<long>(nullable: false),
                    SourceFacilityId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoutPoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoutPoints_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoutPoints_PickingTypes_PickingTypeId",
                        column: x => x.PickingTypeId,
                        principalTable: "PickingTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoutPoints_Facilities_SourceFacilityId",
                        column: x => x.SourceFacilityId,
                        principalTable: "Facilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoutPoints_AbpTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoutPointGoodsDetails",
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
                    GoodsDetailsId = table.Column<long>(nullable: false),
                    RoutPointId = table.Column<long>(nullable: false),
                    Amount = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoutPointGoodsDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoutPointGoodsDetails_GoodsDetails_GoodsDetailsId",
                        column: x => x.GoodsDetailsId,
                        principalTable: "GoodsDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoutPointGoodsDetails_RoutPoints_RoutPointId",
                        column: x => x.RoutPointId,
                        principalTable: "RoutPoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoutSteps_DestinationRoutPointId",
                table: "RoutSteps",
                column: "DestinationRoutPointId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutSteps_SourceRoutPointId",
                table: "RoutSteps",
                column: "SourceRoutPointId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsDetails_ShippingRequestId",
                table: "GoodsDetails",
                column: "ShippingRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodCategories_FatherId",
                table: "GoodCategories",
                column: "FatherId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutPointGoodsDetails_GoodsDetailsId",
                table: "RoutPointGoodsDetails",
                column: "GoodsDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutPointGoodsDetails_RoutPointId",
                table: "RoutPointGoodsDetails",
                column: "RoutPointId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutPoints_CityId",
                table: "RoutPoints",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutPoints_PickingTypeId",
                table: "RoutPoints",
                column: "PickingTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutPoints_SourceFacilityId",
                table: "RoutPoints",
                column: "SourceFacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutPoints_TenantId",
                table: "RoutPoints",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_GoodCategories_GoodCategories_FatherId",
                table: "GoodCategories",
                column: "FatherId",
                principalTable: "GoodCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GoodsDetails_ShippingRequests_ShippingRequestId",
                table: "GoodsDetails",
                column: "ShippingRequestId",
                principalTable: "ShippingRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_RoutSteps_Trailers_AssignedTrailerId",
                table: "RoutSteps",
                column: "AssignedTrailerId",
                principalTable: "Trailers",
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
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_RoutSteps_RoutPoints_SourceRoutPointId",
                table: "RoutSteps",
                column: "SourceRoutPointId",
                principalTable: "RoutPoints",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GoodCategories_GoodCategories_FatherId",
                table: "GoodCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_GoodsDetails_ShippingRequests_ShippingRequestId",
                table: "GoodsDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_RoutSteps_Trailers_AssignedTrailerId",
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

            migrationBuilder.DropTable(
                name: "RoutPointGoodsDetails");

            migrationBuilder.DropTable(
                name: "RoutPoints");

            migrationBuilder.DropIndex(
                name: "IX_RoutSteps_DestinationRoutPointId",
                table: "RoutSteps");

            migrationBuilder.DropIndex(
                name: "IX_RoutSteps_SourceRoutPointId",
                table: "RoutSteps");

            migrationBuilder.DropIndex(
                name: "IX_GoodsDetails_ShippingRequestId",
                table: "GoodsDetails");

            migrationBuilder.DropIndex(
                name: "IX_GoodCategories_FatherId",
                table: "GoodCategories");

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
                name: "NumberOfPackingType",
                table: "GoodsDetails");

            migrationBuilder.DropColumn(
                name: "PackingType",
                table: "GoodsDetails");

            migrationBuilder.DropColumn(
                name: "ShippingRequestId",
                table: "GoodsDetails");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "GoodsDetails");

            migrationBuilder.DropColumn(
                name: "FatherId",
                table: "GoodCategories");

            migrationBuilder.AlterColumn<long>(
                name: "ShippingRequestId",
                table: "RoutSteps",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<long>(
                name: "AssignedTrailerId",
                table: "RoutSteps",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

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

            migrationBuilder.AddForeignKey(
                name: "FK_RoutSteps_Trailers_AssignedTrailerId",
                table: "RoutSteps",
                column: "AssignedTrailerId",
                principalTable: "Trailers",
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
        }
    }
}
