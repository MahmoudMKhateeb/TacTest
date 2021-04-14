using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class AddMobileNoForTenant : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MobileNo",
                table: "AbpTenants",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MobileNo",
                table: "AbpTenants");
        }
    }
}
