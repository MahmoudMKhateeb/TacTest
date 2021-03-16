using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class Invoices : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "InvoiceId",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPrePayed",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InvoicePeriodId",
                table: "AbpTenants",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "InvoicePeriods",
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
                    DisplayName = table.Column<string>(maxLength: 256, nullable: false),
                    Description = table.Column<string>(nullable: true),
                    CreditLimit = table.Column<double>(nullable: false),
                    PeriodType = table.Column<byte>(nullable: false),
                    FreqInterval = table.Column<int>(nullable: false),
                    FreqRelativeInterval = table.Column<byte>(nullable: false),
                    FreqRecurrence = table.Column<string>(nullable: true),
                    NextRunDate = table.Column<DateTime>(nullable: true),
                    ShipperOnlyUsed = table.Column<bool>(nullable: false),
                    Enabled = table.Column<bool>(nullable: false),
                    LastRunDate = table.Column<DateTime>(nullable: true),
                    Cronexpression = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoicePeriods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Invoices",
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
                    DueDate = table.Column<DateTime>(nullable: false),
                    IsPaid = table.Column<bool>(nullable: false),
                    Note = table.Column<string>(nullable: true),
                    Amount = table.Column<decimal>(nullable: true),
                    TotalSumExclVat = table.Column<decimal>(nullable: true),
                    TotalVat = table.Column<decimal>(nullable: true),
                    IsAccountReceivable = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invoices_InvoicePeriods_PeriodId",
                        column: x => x.PeriodId,
                        principalTable: "InvoicePeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Invoices_AbpTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceShippingRequests",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestId = table.Column<long>(nullable: false),
                    InvoiceId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceShippingRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoiceShippingRequests_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvoiceShippingRequests_ShippingRequests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "ShippingRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_InvoicePeriodId",
                table: "AbpTenants",
                column: "InvoicePeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_PeriodId",
                table: "Invoices",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_TenantId",
                table: "Invoices",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceShippingRequests_InvoiceId",
                table: "InvoiceShippingRequests",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceShippingRequests_RequestId",
                table: "InvoiceShippingRequests",
                column: "RequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_AbpTenants_InvoicePeriods_InvoicePeriodId",
                table: "AbpTenants",
                column: "InvoicePeriodId",
                principalTable: "InvoicePeriods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AbpTenants_InvoicePeriods_InvoicePeriodId",
                table: "AbpTenants");

            migrationBuilder.DropTable(
                name: "InvoiceShippingRequests");

            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropTable(
                name: "InvoicePeriods");

            migrationBuilder.DropIndex(
                name: "IX_AbpTenants_InvoicePeriodId",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "InvoiceId",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "IsPrePayed",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "InvoicePeriodId",
                table: "AbpTenants");
        }
    }
}
