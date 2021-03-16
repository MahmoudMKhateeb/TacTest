using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class InvoiceProformaUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeleterUserId",
                table: "InvoicesProforma");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                table: "InvoicesProforma");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "InvoicesProforma");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                table: "InvoicesProforma");

            migrationBuilder.DropColumn(
                name: "LastModifierUserId",
                table: "InvoicesProforma");

            migrationBuilder.AddColumn<long>(
                name: "RequestId",
                table: "InvoicesProforma",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_InvoicesProforma_RequestId",
                table: "InvoicesProforma",
                column: "RequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoicesProforma_ShippingRequests_RequestId",
                table: "InvoicesProforma",
                column: "RequestId",
                principalTable: "ShippingRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvoicesProforma_ShippingRequests_RequestId",
                table: "InvoicesProforma");

            migrationBuilder.DropIndex(
                name: "IX_InvoicesProforma_RequestId",
                table: "InvoicesProforma");

            migrationBuilder.DropColumn(
                name: "RequestId",
                table: "InvoicesProforma");

            migrationBuilder.AddColumn<long>(
                name: "DeleterUserId",
                table: "InvoicesProforma",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                table: "InvoicesProforma",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "InvoicesProforma",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationTime",
                table: "InvoicesProforma",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "LastModifierUserId",
                table: "InvoicesProforma",
                type: "bigint",
                nullable: true);
        }
    }
}
