using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class addTenantinShippingRequestBidrealation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestBids_TenantId",
                table: "ShippingRequestBids",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequestBids_AbpTenants_TenantId",
                table: "ShippingRequestBids",
                column: "TenantId",
                principalTable: "AbpTenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestBids_AbpTenants_TenantId",
                table: "ShippingRequestBids");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequestBids_TenantId",
                table: "ShippingRequestBids");
        }
    }
}
