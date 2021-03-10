using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class RoutePointRemovePikingType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoutPoints_RoutPoints_ParentId",
                table: "RoutPoints");

            migrationBuilder.DropForeignKey(
                name: "FK_RoutPoints_PickingTypes_PickingTypeId",
                table: "RoutPoints");

            migrationBuilder.DropTable(
                name: "PickingTypes");

            migrationBuilder.DropIndex(
                name: "IX_RoutPoints_ParentId",
                table: "RoutPoints");

            migrationBuilder.DropIndex(
                name: "IX_RoutPoints_PickingTypeId",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "PickingTypeId",
                table: "RoutPoints");

            migrationBuilder.AddColumn<byte>(
                name: "PickingType",
                table: "RoutPoints",
                nullable: false,
                defaultValue: (byte)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PickingType",
                table: "RoutPoints");

            migrationBuilder.AddColumn<long>(
                name: "ParentId",
                table: "RoutPoints",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PickingTypeId",
                table: "RoutPoints",
                type: "int",
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

            migrationBuilder.CreateIndex(
                name: "IX_RoutPoints_ParentId",
                table: "RoutPoints",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutPoints_PickingTypeId",
                table: "RoutPoints",
                column: "PickingTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoutPoints_RoutPoints_ParentId",
                table: "RoutPoints",
                column: "ParentId",
                principalTable: "RoutPoints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoutPoints_PickingTypes_PickingTypeId",
                table: "RoutPoints",
                column: "PickingTypeId",
                principalTable: "PickingTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
