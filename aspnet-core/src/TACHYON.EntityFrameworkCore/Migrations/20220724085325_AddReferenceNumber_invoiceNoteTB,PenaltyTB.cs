using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class AddReferenceNumber_invoiceNoteTBPenaltyTB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReferenceNumber",
                table: "Penalties",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceNoteReferenceNumber",
                table: "InvoiceNotes",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReferenceNumber",
                table: "Penalties");

            migrationBuilder.DropColumn(
                name: "InvoiceNoteReferenceNumber",
                table: "InvoiceNotes");
        }
    }
}
