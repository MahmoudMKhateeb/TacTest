using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class add_destination_tenant_to_Penalty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DestinationTenantId",
                table: "Penalties",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Penalties_DestinationTenantId",
                table: "Penalties",
                column: "DestinationTenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Penalties_AbpTenants_DestinationTenantId",
                table: "Penalties",
                column: "DestinationTenantId",
                principalTable: "AbpTenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Penalties_AbpTenants_DestinationTenantId",
                table: "Penalties");

            migrationBuilder.DropIndex(
                name: "IX_Penalties_DestinationTenantId",
                table: "Penalties");

            migrationBuilder.DropColumn(
                name: "DestinationTenantId",
                table: "Penalties");
        }
    }
}
