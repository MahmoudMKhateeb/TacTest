using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class demandupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DemandFileContentType",
                table: "GroupPeriods",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DemandFileName",
                table: "GroupPeriods",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DemandFileContentType",
                table: "GroupPeriods");

            migrationBuilder.DropColumn(
                name: "DemandFileName",
                table: "GroupPeriods");
        }
    }
}
