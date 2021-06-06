using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class CreateCarrierForeignKeyTenantCarriers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_TenantCarriers_CarrierTenantId",
                table: "TenantCarriers",
                column: "CarrierTenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_TenantCarriers_AbpTenants_CarrierTenantId",
                table: "TenantCarriers",
                column: "CarrierTenantId",
                principalTable: "AbpTenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TenantCarriers_AbpTenants_CarrierTenantId",
                table: "TenantCarriers");

            migrationBuilder.DropIndex(
                name: "IX_TenantCarriers_CarrierTenantId",
                table: "TenantCarriers");
        }
    }
}
