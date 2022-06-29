using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class NullableDestComp_PenaltyTB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Penalties_AbpTenants_DestinationTenantId",
                table: "Penalties");

            migrationBuilder.AlterColumn<int>(
                name: "DestinationTenantId",
                table: "Penalties",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

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

            migrationBuilder.AlterColumn<int>(
                name: "DestinationTenantId",
                table: "Penalties",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Penalties_AbpTenants_DestinationTenantId",
                table: "Penalties",
                column: "DestinationTenantId",
                principalTable: "AbpTenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
