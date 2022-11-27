using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class InsuranceCoverage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InsuranceCoverage",
                table: "AbpTenants",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ValueOfGoods",
                table: "AbpTenants",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InsuranceCoverage",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "ValueOfGoods",
                table: "AbpTenants");
        }
    }
}
