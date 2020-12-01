using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class addTenantrealationinShippingRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AddColumn<string>(
            //    name: "Capacity",
            //    table: "Trucks",
            //    nullable: false,
            //    defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequests_AbpTenants_TenantId",
                table: "ShippingRequests",
                column: "TenantId",
                principalTable: "AbpTenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_AbpTenants_TenantId",
                table: "ShippingRequests");

            //migrationBuilder.DropColumn(
            //    name: "Capacity",
            //    table: "Trucks");
        }
    }
}
