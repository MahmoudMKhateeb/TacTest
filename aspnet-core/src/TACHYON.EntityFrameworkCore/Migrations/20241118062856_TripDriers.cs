using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class TripDriers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.CreateTable(
            //     name: "DynamicInvoiceCustomItems",
            //     columns: table => new
            //     {
            //         Id = table.Column<long>(nullable: false)
            //             .Annotation("SqlServer:Identity", "1, 1"),
            //         CreationTime = table.Column<DateTime>(nullable: false),
            //         CreatorUserId = table.Column<long>(nullable: true),
            //         LastModificationTime = table.Column<DateTime>(nullable: true),
            //         LastModifierUserId = table.Column<long>(nullable: true),
            //         IsDeleted = table.Column<bool>(nullable: false),
            //         DeleterUserId = table.Column<long>(nullable: true),
            //         DeletionTime = table.Column<DateTime>(nullable: true),
            //         DynamicInvoiceId = table.Column<long>(nullable: false),
            //         ItemName = table.Column<string>(nullable: false),
            //         Description = table.Column<string>(nullable: false),
            //         VatAmount = table.Column<decimal>(nullable: false),
            //         VatTax = table.Column<decimal>(nullable: false),
            //         TotalAmount = table.Column<decimal>(nullable: false),
            //         Quantity = table.Column<int>(nullable: true),
            //         Price = table.Column<decimal>(nullable: false)
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_DynamicInvoiceCustomItems", x => x.Id);
            //         table.ForeignKey(
            //             name: "FK_DynamicInvoiceCustomItems_DynamicInvoices_DynamicInvoiceId",
            //             column: x => x.DynamicInvoiceId,
            //             principalTable: "DynamicInvoices",
            //             principalColumn: "Id",
            //             onDelete: ReferentialAction.Cascade);
            //     });

            migrationBuilder.CreateTable(
                name: "TripDrivers",
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
                    DriverId = table.Column<long>(nullable: false),
                    DriverStatus = table.Column<int>(nullable: false),
                    TotalWorkingHours = table.Column<int>(nullable: false),
                    DistanceCovered = table.Column<decimal>(nullable: false),
                    Commission = table.Column<decimal>(nullable: false),
                    ShippingRequestTripId = table.Column<int>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    TruckId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TripDrivers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TripDrivers_AbpUsers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TripDrivers_ShippingRequestTrips_ShippingRequestTripId",
                        column: x => x.ShippingRequestTripId,
                        principalTable: "ShippingRequestTrips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TripDrivers_Trucks_TruckId",
                        column: x => x.TruckId,
                        principalTable: "Trucks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            // migrationBuilder.CreateIndex(
            //     name: "IX_DynamicInvoiceCustomItems_DynamicInvoiceId",
            //     table: "DynamicInvoiceCustomItems",
            //     column: "DynamicInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_TripDrivers_DriverId",
                table: "TripDrivers",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_TripDrivers_ShippingRequestTripId",
                table: "TripDrivers",
                column: "ShippingRequestTripId");

            migrationBuilder.CreateIndex(
                name: "IX_TripDrivers_TruckId",
                table: "TripDrivers",
                column: "TruckId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.DropTable(
            //     name: "DynamicInvoiceCustomItems");

            migrationBuilder.DropTable(
                name: "TripDrivers");
        }
    }
}
