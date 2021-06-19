using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class CreateSubmitInvoices : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "Channel",
                table: "Invoices",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.CreateTable(
                name: "SubmitInvoices",
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
                    ReferencNumber = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    PeriodId = table.Column<int>(nullable: false),
                    Channel = table.Column<byte>(nullable: false),
                    SubTotalAmount = table.Column<decimal>(nullable: false),
                    TotalAmount = table.Column<decimal>(nullable: false),
                    VatAmount = table.Column<decimal>(nullable: false),
                    TaxVat = table.Column<decimal>(nullable: false),
                    DocumentId = table.Column<Guid>(nullable: true),
                    DocumentName = table.Column<string>(nullable: true),
                    DocumentContentType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubmitInvoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubmitInvoices_InvoicePeriods_PeriodId",
                        column: x => x.PeriodId,
                        principalTable: "InvoicePeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubmitInvoices_AbpTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubmitInvoiceTrips",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubmitId = table.Column<long>(nullable: false),
                    TripId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubmitInvoiceTrips", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubmitInvoiceTrips_SubmitInvoices_SubmitId",
                        column: x => x.SubmitId,
                        principalTable: "SubmitInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubmitInvoiceTrips_ShippingRequestTrips_TripId",
                        column: x => x.TripId,
                        principalTable: "ShippingRequestTrips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubmitInvoices_PeriodId",
                table: "SubmitInvoices",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_SubmitInvoices_TenantId",
                table: "SubmitInvoices",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SubmitInvoiceTrips_SubmitId",
                table: "SubmitInvoiceTrips",
                column: "SubmitId");

            migrationBuilder.CreateIndex(
                name: "IX_SubmitInvoiceTrips_TripId",
                table: "SubmitInvoiceTrips",
                column: "TripId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubmitInvoiceTrips");

            migrationBuilder.DropTable(
                name: "SubmitInvoices");

            migrationBuilder.DropColumn(
                name: "Channel",
                table: "Invoices");
        }
    }
}
