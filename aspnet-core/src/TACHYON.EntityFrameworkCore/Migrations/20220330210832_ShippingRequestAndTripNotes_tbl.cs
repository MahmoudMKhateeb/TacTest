using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class ShippingRequestAndTripNotes_tbl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<int>(
                name: "NoteId",
                table: "DocumentFiles",
                nullable: true);

           

            migrationBuilder.CreateTable(
                name: "ShippingRequestAndTripNotes",
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
                    Note = table.Column<string>(nullable: true),
                    TripId = table.Column<int>(nullable: true),
                    ShippingRequetId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    Visibility = table.Column<byte>(nullable: false),
                    IsPrintedByWabillInvoice = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingRequestAndTripNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingRequestAndTripNotes_ShippingRequests_ShippingRequetId",
                        column: x => x.ShippingRequetId,
                        principalTable: "ShippingRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShippingRequestAndTripNotes_AbpTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShippingRequestAndTripNotes_ShippingRequestTrips_TripId",
                        column: x => x.TripId,
                        principalTable: "ShippingRequestTrips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

           
            migrationBuilder.CreateIndex(
                name: "IX_DocumentFiles_NoteId",
                table: "DocumentFiles",
                column: "NoteId");

          
            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestAndTripNotes_ShippingRequetId",
                table: "ShippingRequestAndTripNotes",
                column: "ShippingRequetId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestAndTripNotes_TenantId",
                table: "ShippingRequestAndTripNotes",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestAndTripNotes_TripId",
                table: "ShippingRequestAndTripNotes",
                column: "TripId");

            
            migrationBuilder.AddForeignKey(
                name: "FK_DocumentFiles_ShippingRequestAndTripNotes_NoteId",
                table: "DocumentFiles",
                column: "NoteId",
                principalTable: "ShippingRequestAndTripNotes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AbpUsers_DriverLicenseTypes_DriverLicenseTypeId",
                table: "AbpUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_DocumentFiles_ShippingRequestAndTripNotes_NoteId",
                table: "DocumentFiles");

            migrationBuilder.DropTable(
                name: "ShippingRequestAndTripNotes");

            migrationBuilder.DropIndex(
                name: "IX_DocumentFiles_NoteId",
                table: "DocumentFiles");

          
            migrationBuilder.DropColumn(
                name: "NoteId",
                table: "DocumentFiles");

           
        }
    }
}
