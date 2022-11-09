using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class release_2_4_3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ItemName",
                table: "InvoiceNoteItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocumentContentType",
                table: "ActorSubmitInvoices",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DocumentId",
                table: "ActorSubmitInvoices",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocumentName",
                table: "ActorSubmitInvoices",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ItemName",
                table: "InvoiceNoteItems");

            migrationBuilder.DropColumn(
                name: "DocumentContentType",
                table: "ActorSubmitInvoices");

            migrationBuilder.DropColumn(
                name: "DocumentId",
                table: "ActorSubmitInvoices");

            migrationBuilder.DropColumn(
                name: "DocumentName",
                table: "ActorSubmitInvoices");
        }
    }
}
