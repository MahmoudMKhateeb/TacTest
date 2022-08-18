using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class SetNallableTenantId_PenaltyTB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Penalties_AbpTenants_TenantId",
                table: "Penalties");

            migrationBuilder.AlterColumn<int>(
                name: "TenantId",
                table: "Penalties",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Penalties_AbpTenants_TenantId",
                table: "Penalties",
                column: "TenantId",
                principalTable: "AbpTenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Penalties_AbpTenants_TenantId",
                table: "Penalties");

            migrationBuilder.AlterColumn<int>(
                name: "TenantId",
                table: "Penalties",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Penalties_AbpTenants_TenantId",
                table: "Penalties",
                column: "TenantId",
                principalTable: "AbpTenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
