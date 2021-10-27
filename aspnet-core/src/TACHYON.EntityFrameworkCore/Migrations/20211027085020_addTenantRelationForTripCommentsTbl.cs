using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class addTenantRelationForTripCommentsTbl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTripAccidentComments_TenantId",
                table: "ShippingRequestTripAccidentComments",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequestTripAccidentComments_AbpTenants_TenantId",
                table: "ShippingRequestTripAccidentComments",
                column: "TenantId",
                principalTable: "AbpTenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestTripAccidentComments_AbpTenants_TenantId",
                table: "ShippingRequestTripAccidentComments");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequestTripAccidentComments_TenantId",
                table: "ShippingRequestTripAccidentComments");
        }
    }
}