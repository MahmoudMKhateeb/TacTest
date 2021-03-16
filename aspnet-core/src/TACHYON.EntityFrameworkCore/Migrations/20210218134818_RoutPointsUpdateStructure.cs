using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class RoutPointsUpdateStructure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GoodsDetails_ShippingRequests_ShippingRequestId",
                table: "GoodsDetails");

            migrationBuilder.DropTable(
                name: "RoutPointGoodsDetails");

            migrationBuilder.DropIndex(
                name: "IX_RoutSteps_TenantId",
                table: "RoutSteps");

            migrationBuilder.DropColumn(
                name: "Adress",
                table: "Ports");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "GoodsDetails");

            migrationBuilder.DropColumn(
                name: "Adress",
                table: "Facilities");

            migrationBuilder.AddColumn<long>(
                name: "ShippingRequestId",
                table: "RoutPoints",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Ports",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<long>(
                name: "ShippingRequestId",
                table: "GoodsDetails",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

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

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Facilities",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_RoutPoints_ShippingRequestId",
                table: "RoutPoints",
                column: "ShippingRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsDetails_RoutPointId",
                table: "GoodsDetails",
                column: "RoutPointId");

            migrationBuilder.AddForeignKey(
                name: "FK_GoodsDetails_RoutPoints_RoutPointId",
                table: "GoodsDetails",
                column: "RoutPointId",
                principalTable: "RoutPoints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GoodsDetails_ShippingRequests_ShippingRequestId",
                table: "GoodsDetails",
                column: "ShippingRequestId",
                principalTable: "ShippingRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoutPoints_ShippingRequests_ShippingRequestId",
                table: "RoutPoints",
                column: "ShippingRequestId",
                principalTable: "ShippingRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GoodsDetails_RoutPoints_RoutPointId",
                table: "GoodsDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_GoodsDetails_ShippingRequests_ShippingRequestId",
                table: "GoodsDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_RoutPoints_ShippingRequests_ShippingRequestId",
                table: "RoutPoints");

            migrationBuilder.DropIndex(
                name: "IX_RoutPoints_ShippingRequestId",
                table: "RoutPoints");

            migrationBuilder.DropIndex(
                name: "IX_GoodsDetails_RoutPointId",
                table: "GoodsDetails");

            migrationBuilder.DropColumn(
                name: "ShippingRequestId",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Ports");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "GoodsDetails");

            migrationBuilder.DropColumn(
                name: "RoutPointId",
                table: "GoodsDetails");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Facilities");

            migrationBuilder.AddColumn<string>(
                name: "Adress",
                table: "Ports",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<long>(
                name: "ShippingRequestId",
                table: "GoodsDetails",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalAmount",
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

            migrationBuilder.CreateTable(
                name: "RoutPointGoodsDetails",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GoodsDetailsId = table.Column<long>(type: "bigint", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    RoutPointId = table.Column<long>(type: "bigint", nullable: false)
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
                name: "IX_RoutSteps_TenantId",
                table: "RoutSteps",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutPointGoodsDetails_GoodsDetailsId",
                table: "RoutPointGoodsDetails",
                column: "GoodsDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutPointGoodsDetails_RoutPointId",
                table: "RoutPointGoodsDetails",
                column: "RoutPointId");

            migrationBuilder.AddForeignKey(
                name: "FK_GoodsDetails_ShippingRequests_ShippingRequestId",
                table: "GoodsDetails",
                column: "ShippingRequestId",
                principalTable: "ShippingRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
