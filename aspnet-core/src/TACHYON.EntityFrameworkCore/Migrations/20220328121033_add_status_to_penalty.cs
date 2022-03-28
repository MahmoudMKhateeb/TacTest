using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class add_status_to_penalty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RejectReason",
                table: "PenaltyComplaints",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Penalties",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RejectReason",
                table: "PenaltyComplaints");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Penalties");
        }
    }
}
