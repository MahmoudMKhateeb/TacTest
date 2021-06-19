using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class AddNewFiledsToSubmitInvoices : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RejectedReason",
                table: "SubmitInvoices",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "Status",
                table: "SubmitInvoices",
                nullable: false,
                defaultValue: (byte)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RejectedReason",
                table: "SubmitInvoices");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "SubmitInvoices");
        }
    }
}
