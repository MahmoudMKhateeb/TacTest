using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class BidChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsClosedBid",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "isCancledBid",
                table: "ShippingRequests");

            migrationBuilder.AddColumn<int>(
                name: "ShippingRequestBidStatusId",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsAccepted",
                table: "ShippingRequestBids",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsRejected",
                table: "ShippingRequestBids",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ShippingRequestBidStatuses",
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
                    table.PrimaryKey("PK_ShippingRequestBidStatuses", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequests_ShippingRequestBidStatusId",
                table: "ShippingRequests",
                column: "ShippingRequestBidStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequests_ShippingRequestBidStatuses_ShippingRequestBidStatusId",
                table: "ShippingRequests",
                column: "ShippingRequestBidStatusId",
                principalTable: "ShippingRequestBidStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_ShippingRequestBidStatuses_ShippingRequestBidStatusId",
                table: "ShippingRequests");

            migrationBuilder.DropTable(
                name: "ShippingRequestBidStatuses");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequests_ShippingRequestBidStatusId",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "ShippingRequestBidStatusId",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "IsRejected",
                table: "ShippingRequestBids");

            migrationBuilder.AddColumn<bool>(
                name: "IsClosedBid",
                table: "ShippingRequests",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isCancledBid",
                table: "ShippingRequests",
                type: "bit",
                nullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsAccepted",
                table: "ShippingRequestBids",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool));
        }
    }
}
