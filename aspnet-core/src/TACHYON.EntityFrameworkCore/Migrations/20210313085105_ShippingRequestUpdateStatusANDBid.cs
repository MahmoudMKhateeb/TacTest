using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class ShippingRequestUpdateStatusANDBid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_ShippingRequestBidStatuses_ShippingRequestBidStatusId",
                table: "ShippingRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_ShippingRequestStatuses_ShippingRequestStatusId",
                table: "ShippingRequests");

            migrationBuilder.DropTable(
                name: "ShippingRequestBidStatuses");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequests_ShippingRequestBidStatusId",
                table: "ShippingRequests");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequests_ShippingRequestStatusId",
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

            migrationBuilder.AddColumn<byte>(
                name: "BidStatus",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "Status",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: (byte)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BidStatus",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "ShippingRequests");

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
                name: "IX_ShippingRequests_ShippingRequestBidStatusId",
                table: "ShippingRequests",
                column: "ShippingRequestBidStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequests_ShippingRequestStatusId",
                table: "ShippingRequests",
                column: "ShippingRequestStatusId");

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
