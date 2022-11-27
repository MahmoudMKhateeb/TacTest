using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class _1213 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AddColumn<string>(
            //    name: "SplitInvoiceFlag",
            //    table: "ShippingRequestTrips",
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "SplitInvoiceFlag",
            //    table: "ShippingRequests",
            //    nullable: true);

            migrationBuilder.CreateTable(
                name: "DynamicInvoices",
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
                    CreditTenantId = table.Column<int>(nullable: true),
                    DebitTenantId = table.Column<int>(nullable: true),
                    InvoiceId = table.Column<long>(nullable: true),
                    Notes = table.Column<string>(nullable: true),
                    SubmitInvoiceId = table.Column<long>(nullable: true),
                    SubTotalAmount = table.Column<decimal>(nullable: false),
                    VatAmount = table.Column<decimal>(nullable: false),
                    TotalAmount = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DynamicInvoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DynamicInvoices_AbpTenants_CreditTenantId",
                        column: x => x.CreditTenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DynamicInvoices_AbpTenants_DebitTenantId",
                        column: x => x.DebitTenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DynamicInvoices_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DynamicInvoices_SubmitInvoices_SubmitInvoiceId",
                        column: x => x.SubmitInvoiceId,
                        principalTable: "SubmitInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DynamicInvoiceItems",
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
                    TripId = table.Column<int>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Price = table.Column<decimal>(nullable: false),
                    OriginCityId = table.Column<int>(nullable: true),
                    DestinationCityId = table.Column<int>(nullable: true),
                    TruckId = table.Column<long>(nullable: true),
                    WorkDate = table.Column<DateTime>(nullable: true),
                    Quantity = table.Column<int>(nullable: true),
                    ContainerNumber = table.Column<string>(nullable: true),
                    DynamicInvoiceId = table.Column<long>(nullable: false),
                    VatAmount = table.Column<decimal>(nullable: false),
                    TotalAmount = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DynamicInvoiceItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DynamicInvoiceItems_Cities_DestinationCityId",
                        column: x => x.DestinationCityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DynamicInvoiceItems_DynamicInvoices_DynamicInvoiceId",
                        column: x => x.DynamicInvoiceId,
                        principalTable: "DynamicInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DynamicInvoiceItems_Cities_OriginCityId",
                        column: x => x.OriginCityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DynamicInvoiceItems_ShippingRequestTrips_TripId",
                        column: x => x.TripId,
                        principalTable: "ShippingRequestTrips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DynamicInvoiceItems_Trucks_TruckId",
                        column: x => x.TruckId,
                        principalTable: "Trucks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DynamicInvoiceItems_DestinationCityId",
                table: "DynamicInvoiceItems",
                column: "DestinationCityId");

            migrationBuilder.CreateIndex(
                name: "IX_DynamicInvoiceItems_DynamicInvoiceId",
                table: "DynamicInvoiceItems",
                column: "DynamicInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_DynamicInvoiceItems_OriginCityId",
                table: "DynamicInvoiceItems",
                column: "OriginCityId");

            migrationBuilder.CreateIndex(
                name: "IX_DynamicInvoiceItems_TripId",
                table: "DynamicInvoiceItems",
                column: "TripId");

            migrationBuilder.CreateIndex(
                name: "IX_DynamicInvoiceItems_TruckId",
                table: "DynamicInvoiceItems",
                column: "TruckId");

            migrationBuilder.CreateIndex(
                name: "IX_DynamicInvoices_CreditTenantId",
                table: "DynamicInvoices",
                column: "CreditTenantId");

            migrationBuilder.CreateIndex(
                name: "IX_DynamicInvoices_DebitTenantId",
                table: "DynamicInvoices",
                column: "DebitTenantId");

            migrationBuilder.CreateIndex(
                name: "IX_DynamicInvoices_InvoiceId",
                table: "DynamicInvoices",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_DynamicInvoices_SubmitInvoiceId",
                table: "DynamicInvoices",
                column: "SubmitInvoiceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DynamicInvoiceItems");

            migrationBuilder.DropTable(
                name: "DynamicInvoices");

            migrationBuilder.DropColumn(
                name: "SplitInvoiceFlag",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "SplitInvoiceFlag",
                table: "ShippingRequests");
        }
    }
}
