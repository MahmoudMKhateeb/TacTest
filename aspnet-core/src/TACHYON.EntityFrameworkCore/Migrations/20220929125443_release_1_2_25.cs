using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class release_1_2_25 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Penalties_AbpTenants_TenantId",
                table: "Penalties");

            migrationBuilder.AddColumn<string>(
                name: "InternalTruckId",
                table: "Trucks",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AcceptanceDate",
                table: "SubmitInvoices",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentDate",
                table: "SubmitInvoices",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SubmittedDate",
                table: "SubmitInvoices",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TenantId",
                table: "Penalties",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "InvoiceFlag",
                table: "Penalties",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReferenceNumber",
                table: "Penalties",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentDate",
                table: "Invoices",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Invoices",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceNoteReferenceNumber",
                table: "InvoiceNotes",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SubmitInvoiceNumber",
                table: "InvoiceNotes",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PenaltyItems",
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
                    PenaltyId = table.Column<int>(nullable: false),
                    ShippingRequestTripId = table.Column<int>(nullable: true),
                    ItemPrice = table.Column<decimal>(nullable: false),
                    ItemTotalAmountPostVat = table.Column<decimal>(nullable: false),
                    VatAmount = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PenaltyItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PenaltyItems_Penalties_PenaltyId",
                        column: x => x.PenaltyId,
                        principalTable: "Penalties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PenaltyItems_ShippingRequestTrips_ShippingRequestTripId",
                        column: x => x.ShippingRequestTripId,
                        principalTable: "ShippingRequestTrips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ServiceAreas",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    CityId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceAreas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceAreas_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShippingRequestDestinationCities",
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
                    ShippingRequestId = table.Column<long>(nullable: false),
                    CityId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingRequestDestinationCities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingRequestDestinationCities_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShippingRequestDestinationCities_ShippingRequests_ShippingRequestId",
                        column: x => x.ShippingRequestId,
                        principalTable: "ShippingRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PenaltyItems_PenaltyId",
                table: "PenaltyItems",
                column: "PenaltyId");

            migrationBuilder.CreateIndex(
                name: "IX_PenaltyItems_ShippingRequestTripId",
                table: "PenaltyItems",
                column: "ShippingRequestTripId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceAreas_CityId",
                table: "ServiceAreas",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestDestinationCities_CityId",
                table: "ShippingRequestDestinationCities",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestDestinationCities_ShippingRequestId",
                table: "ShippingRequestDestinationCities",
                column: "ShippingRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Facilities_AbpTenants_TenantId",
                table: "Facilities",
                column: "TenantId",
                principalTable: "AbpTenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Penalties_AbpTenants_TenantId",
                table: "Penalties",
                column: "TenantId",
                principalTable: "AbpTenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Receivers_AbpTenants_TenantId",
                table: "Receivers",
                column: "TenantId",
                principalTable: "AbpTenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Facilities_AbpTenants_TenantId",
                table: "Facilities");

            migrationBuilder.DropForeignKey(
                name: "FK_Penalties_AbpTenants_TenantId",
                table: "Penalties");

            migrationBuilder.DropForeignKey(
                name: "FK_Receivers_AbpTenants_TenantId",
                table: "Receivers");

            migrationBuilder.DropTable(
                name: "PenaltyItems");

            migrationBuilder.DropTable(
                name: "ServiceAreas");

            migrationBuilder.DropTable(
                name: "ShippingRequestDestinationCities");

            migrationBuilder.DropColumn(
                name: "InternalTruckId",
                table: "Trucks");

            migrationBuilder.DropColumn(
                name: "AcceptanceDate",
                table: "SubmitInvoices");

            migrationBuilder.DropColumn(
                name: "PaymentDate",
                table: "SubmitInvoices");

            migrationBuilder.DropColumn(
                name: "SubmittedDate",
                table: "SubmitInvoices");

            migrationBuilder.DropColumn(
                name: "InvoiceFlag",
                table: "Penalties");

            migrationBuilder.DropColumn(
                name: "ReferenceNumber",
                table: "Penalties");

            migrationBuilder.DropColumn(
                name: "PaymentDate",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "InvoiceNoteReferenceNumber",
                table: "InvoiceNotes");

            migrationBuilder.DropColumn(
                name: "SubmitInvoiceNumber",
                table: "InvoiceNotes");

            migrationBuilder.AlterColumn<int>(
                name: "TenantId",
                table: "Penalties",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Penalties_AbpTenants_TenantId",
                table: "Penalties",
                column: "TenantId",
                principalTable: "AbpTenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
