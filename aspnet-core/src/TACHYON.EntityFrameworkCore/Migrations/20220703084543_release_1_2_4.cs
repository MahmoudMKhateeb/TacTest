using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class release_1_2_4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Penalties_AbpTenants_DestinationTenantId",
                table: "Penalties");

            migrationBuilder.AddColumn<bool>(
                name: "IsAppearAmount",
                table: "Vases",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "DestinationTenantId",
                table: "Penalties",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<decimal>(
                name: "CommissionPercentageOrAddValue",
                table: "Penalties",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsDrafted",
                table: "Penalties",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PenaltyComplaintId",
                table: "Penalties",
                nullable: true);

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

            migrationBuilder.DropColumn(
                name: "IsAppearAmount",
                table: "Vases");

            migrationBuilder.DropColumn(
                name: "CommissionPercentageOrAddValue",
                table: "Penalties");

            migrationBuilder.DropColumn(
                name: "IsDrafted",
                table: "Penalties");

            migrationBuilder.DropColumn(
                name: "PenaltyComplaintId",
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
