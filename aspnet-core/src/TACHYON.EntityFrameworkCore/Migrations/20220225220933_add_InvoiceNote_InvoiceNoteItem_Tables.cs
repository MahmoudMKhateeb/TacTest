using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class add_InvoiceNote_InvoiceNoteItem_Tables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Invoices_InvoiceNumber",
                table: "Invoices");

            migrationBuilder.AlterColumn<long>(
                name: "InvoiceNumber",
                table: "Invoices",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "InvoiceNotes",
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
                    NoteType = table.Column<byte>(nullable: false),
                    Status = table.Column<byte>(nullable: false),
                    Remarks = table.Column<string>(nullable: true),
                    WaybillNumber = table.Column<string>(nullable: true),
                    VatAmount = table.Column<decimal>(nullable: false),
                    Price = table.Column<decimal>(nullable: false),
                    TotalValue = table.Column<decimal>(nullable: false),
                    ReferanceNumber = table.Column<string>(nullable: true),
                    InvoiceNumber = table.Column<long>(nullable: false),
                    VoidType = table.Column<int>(nullable: false),
                    IsManual = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoiceNotes_AbpTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceNoteItem",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceNoteId = table.Column<long>(nullable: false),
                    TripId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceNoteItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoiceNoteItem_InvoiceNotes_InvoiceNoteId",
                        column: x => x.InvoiceNoteId,
                        principalTable: "InvoiceNotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvoiceNoteItem_ShippingRequestTrips_TripId",
                        column: x => x.TripId,
                        principalTable: "ShippingRequestTrips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_InvoiceNumber",
                table: "Invoices",
                column: "InvoiceNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceNoteItem_InvoiceNoteId",
                table: "InvoiceNoteItem",
                column: "InvoiceNoteId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceNoteItem_TripId",
                table: "InvoiceNoteItem",
                column: "TripId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceNotes_TenantId",
                table: "InvoiceNotes",
                column: "TenantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InvoiceNoteItem");

            migrationBuilder.DropTable(
                name: "InvoiceNotes");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_InvoiceNumber",
                table: "Invoices");

            migrationBuilder.AlterColumn<long>(
                name: "InvoiceNumber",
                table: "Invoices",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_InvoiceNumber",
                table: "Invoices",
                column: "InvoiceNumber",
                unique: true,
                filter: "[InvoiceNumber] IS NOT NULL");
        }
    }
}
