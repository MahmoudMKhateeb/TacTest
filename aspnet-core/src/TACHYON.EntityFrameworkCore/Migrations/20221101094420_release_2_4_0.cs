using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class release_2_4_0 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "DedicatedKPI",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "KPI",
                table: "DedicatedShippingRequestTrucks",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CR",
                table: "Actors",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Actors",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "VatCertificate",
                table: "Actors",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DedicatedDynamicInvoices",
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
                    InvoiceAccountType = table.Column<byte>(nullable: false),
                    ShippingRequestId = table.Column<long>(nullable: false),
                    SubTotalAmount = table.Column<decimal>(nullable: false),
                    TaxVat = table.Column<decimal>(nullable: false),
                    VatAmount = table.Column<decimal>(nullable: false),
                    TotalAmount = table.Column<decimal>(nullable: false),
                    InvoiceId = table.Column<long>(nullable: true),
                    SubmitInvoiceId = table.Column<long>(nullable: true),
                    Notes = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DedicatedDynamicInvoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DedicatedDynamicInvoices_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DedicatedDynamicInvoices_ShippingRequests_ShippingRequestId",
                        column: x => x.ShippingRequestId,
                        principalTable: "ShippingRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DedicatedDynamicInvoices_SubmitInvoices_SubmitInvoiceId",
                        column: x => x.SubmitInvoiceId,
                        principalTable: "SubmitInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DedicatedDynamicInvoices_AbpTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DedicatedShippingRequestTruckAttendances",
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
                    DedicatedShippingRequestTruckId = table.Column<long>(nullable: false),
                    AttendanceDate = table.Column<DateTime>(nullable: false),
                    AttendaceStatus = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DedicatedShippingRequestTruckAttendances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DedicatedShippingRequestTruckAttendances_DedicatedShippingRequestTrucks_DedicatedShippingRequestTruckId",
                        column: x => x.DedicatedShippingRequestTruckId,
                        principalTable: "DedicatedShippingRequestTrucks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DedicatedDynamicInvoiceItems",
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
                    DedicatedDynamicInvoiceId = table.Column<long>(nullable: false),
                    DedicatedShippingRequestTruckId = table.Column<long>(nullable: false),
                    NumberOfDays = table.Column<int>(nullable: false),
                    PricePerDay = table.Column<decimal>(nullable: false),
                    WorkingDayType = table.Column<byte>(nullable: false),
                    ItemSubTotalAmount = table.Column<decimal>(nullable: false),
                    VatAmount = table.Column<decimal>(nullable: false),
                    TaxVat = table.Column<decimal>(nullable: false),
                    ItemTotalAmount = table.Column<decimal>(nullable: false),
                    Remarks = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DedicatedDynamicInvoiceItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DedicatedDynamicInvoiceItems_DedicatedDynamicInvoices_DedicatedDynamicInvoiceId",
                        column: x => x.DedicatedDynamicInvoiceId,
                        principalTable: "DedicatedDynamicInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DedicatedDynamicInvoiceItems_DedicatedShippingRequestTrucks_DedicatedShippingRequestTruckId",
                        column: x => x.DedicatedShippingRequestTruckId,
                        principalTable: "DedicatedShippingRequestTrucks",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DedicatedDynamicInvoiceItems_DedicatedDynamicInvoiceId",
                table: "DedicatedDynamicInvoiceItems",
                column: "DedicatedDynamicInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_DedicatedDynamicInvoiceItems_DedicatedShippingRequestTruckId",
                table: "DedicatedDynamicInvoiceItems",
                column: "DedicatedShippingRequestTruckId");

            migrationBuilder.CreateIndex(
                name: "IX_DedicatedDynamicInvoices_InvoiceId",
                table: "DedicatedDynamicInvoices",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_DedicatedDynamicInvoices_ShippingRequestId",
                table: "DedicatedDynamicInvoices",
                column: "ShippingRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_DedicatedDynamicInvoices_SubmitInvoiceId",
                table: "DedicatedDynamicInvoices",
                column: "SubmitInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_DedicatedDynamicInvoices_TenantId",
                table: "DedicatedDynamicInvoices",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_DedicatedShippingRequestTruckAttendances_DedicatedShippingRequestTruckId",
                table: "DedicatedShippingRequestTruckAttendances",
                column: "DedicatedShippingRequestTruckId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DedicatedDynamicInvoiceItems");

            migrationBuilder.DropTable(
                name: "DedicatedShippingRequestTruckAttendances");

            migrationBuilder.DropTable(
                name: "DedicatedDynamicInvoices");

            migrationBuilder.DropColumn(
                name: "DedicatedKPI",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "KPI",
                table: "DedicatedShippingRequestTrucks");

            migrationBuilder.DropColumn(
                name: "CR",
                table: "Actors");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Actors");

            migrationBuilder.DropColumn(
                name: "VatCertificate",
                table: "Actors");
        }
    }
}
