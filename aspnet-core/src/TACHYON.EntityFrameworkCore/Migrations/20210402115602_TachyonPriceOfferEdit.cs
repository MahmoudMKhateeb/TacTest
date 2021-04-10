using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class TachyonPriceOfferEdit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TachyonPriceOffers_AbpTenants_TenantCarrirerId",
                table: "TachyonPriceOffers");

            migrationBuilder.DropIndex(
                name: "IX_TachyonPriceOffers_TenantCarrirerId",
                table: "TachyonPriceOffers");

            migrationBuilder.DropColumn(
                name: "TenantCarrirerId",
                table: "TachyonPriceOffers");

            migrationBuilder.CreateIndex(
                name: "IX_TachyonPriceOffers_CarrirerTenantId",
                table: "TachyonPriceOffers",
                column: "CarrirerTenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_TachyonPriceOffers_AbpTenants_CarrirerTenantId",
                table: "TachyonPriceOffers",
                column: "CarrirerTenantId",
                principalTable: "AbpTenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TachyonPriceOffers_AbpTenants_CarrirerTenantId",
                table: "TachyonPriceOffers");

            migrationBuilder.DropIndex(
                name: "IX_TachyonPriceOffers_CarrirerTenantId",
                table: "TachyonPriceOffers");

            migrationBuilder.AddColumn<int>(
                name: "TenantCarrirerId",
                table: "TachyonPriceOffers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TachyonPriceOffers_TenantCarrirerId",
                table: "TachyonPriceOffers",
                column: "TenantCarrirerId");

            migrationBuilder.AddForeignKey(
                name: "FK_TachyonPriceOffers_AbpTenants_TenantCarrirerId",
                table: "TachyonPriceOffers",
                column: "TenantCarrirerId",
                principalTable: "AbpTenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
