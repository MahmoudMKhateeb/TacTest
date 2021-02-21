using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class CarrierGroupRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InvoiceId",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "TotalSumExclVat",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "TotalVat",
                table: "Invoices");

            migrationBuilder.AddColumn<bool>(
                name: "IsCarrierHaveInvoice",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsShipperHaveInvoice",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountWithTaxVat",
                table: "Invoices",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxVat",
                table: "Invoices",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "GroupPeriods",
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
                    PeriodId = table.Column<int>(nullable: false),
                    IsDemand = table.Column<bool>(nullable: false),
                    Note = table.Column<string>(nullable: true),
                    AmountWithTaxVat = table.Column<decimal>(nullable: true),
                    Amount = table.Column<decimal>(nullable: true),
                    TaxVat = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupPeriods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupPeriods_InvoicePeriods_PeriodId",
                        column: x => x.PeriodId,
                        principalTable: "InvoicePeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupPeriods_AbpTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupShippingRequests",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestId = table.Column<long>(nullable: false),
                    GroupId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupShippingRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupShippingRequests_GroupPeriods_GroupId",
                        column: x => x.GroupId,
                        principalTable: "GroupPeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupShippingRequests_ShippingRequests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "ShippingRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroupPeriods_PeriodId",
                table: "GroupPeriods",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupPeriods_TenantId",
                table: "GroupPeriods",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupShippingRequests_GroupId",
                table: "GroupShippingRequests",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupShippingRequests_RequestId",
                table: "GroupShippingRequests",
                column: "RequestId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroupShippingRequests");

            migrationBuilder.DropTable(
                name: "GroupPeriods");

            migrationBuilder.DropColumn(
                name: "IsCarrierHaveInvoice",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "IsShipperHaveInvoice",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "AmountWithTaxVat",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "TaxVat",
                table: "Invoices");

            migrationBuilder.AddColumn<long>(
                name: "InvoiceId",
                table: "ShippingRequests",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalSumExclVat",
                table: "Invoices",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalVat",
                table: "Invoices",
                type: "decimal(18,2)",
                nullable: true);
        }
    }
}
