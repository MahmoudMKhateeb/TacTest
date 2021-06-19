using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class AddContractNumberToTenant : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CancelReason",
                table: "ShippingRequests",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContractNumber",
                table: "AbpTenants",
                maxLength: 12,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_ContractNumber",
                table: "AbpTenants",
                column: "ContractNumber",
                unique: true,
                filter: "[ContractNumber] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AbpTenants_ContractNumber",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "CancelReason",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "ContractNumber",
                table: "AbpTenants");
        }
    }
}
