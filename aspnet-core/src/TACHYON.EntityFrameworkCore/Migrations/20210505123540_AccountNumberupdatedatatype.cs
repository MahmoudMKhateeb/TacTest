using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class AccountNumberupdatedatatype : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AbpTenants_AccountNumber",
                table: "AbpTenants");

            migrationBuilder.AlterColumn<int>(
                name: "AccountNumber",
                table: "AbpTenants",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_AccountNumber",
                table: "AbpTenants",
                column: "AccountNumber",
                unique: true,
                filter: "[AccountNumber] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AbpTenants_AccountNumber",
                table: "AbpTenants");

            migrationBuilder.AlterColumn<int>(
                name: "AccountNumber",
                table: "AbpTenants",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_AccountNumber",
                table: "AbpTenants",
                column: "AccountNumber",
                unique: true);
        }
    }
}
