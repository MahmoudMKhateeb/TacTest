using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class set_ShipperId_to_DestinationTenantId_TmsPricePackageTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TmsPricePackages_AbpTenants_ShipperId",
                table: "TmsPricePackages");

            migrationBuilder.DropIndex(
                name: "IX_TmsPricePackages_ShipperId",
                table: "TmsPricePackages");

            migrationBuilder.DropColumn(
                name: "DirectRequestCommission",
                table: "TmsPricePackages");

            migrationBuilder.DropColumn(
                name: "DirectRequestPrice",
                table: "TmsPricePackages");

            migrationBuilder.DropColumn(
                name: "DirectRequestTotalPrice",
                table: "TmsPricePackages");

            migrationBuilder.DropColumn(
                name: "ShipperId",
                table: "TmsPricePackages");

            migrationBuilder.DropColumn(
                name: "TachyonManageCommission",
                table: "TmsPricePackages");

            migrationBuilder.DropColumn(
                name: "TachyonManagePrice",
                table: "TmsPricePackages");

            migrationBuilder.DropColumn(
                name: "TachyonManageTotalPrice",
                table: "TmsPricePackages");

            migrationBuilder.AddColumn<int>(
                name: "DestinationTenantId",
                table: "TmsPricePackages",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPrice",
                table: "TmsPricePackages",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<string>(
                name: "ReferencNumber",
                table: "ActorSubmitInvoices",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InvoiceNumber",
                table: "ActorInvoices",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TmsPricePackages_DestinationTenantId",
                table: "TmsPricePackages",
                column: "DestinationTenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_TmsPricePackages_AbpTenants_DestinationTenantId",
                table: "TmsPricePackages",
                column: "DestinationTenantId",
                principalTable: "AbpTenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TmsPricePackages_AbpTenants_DestinationTenantId",
                table: "TmsPricePackages");

            migrationBuilder.DropIndex(
                name: "IX_TmsPricePackages_DestinationTenantId",
                table: "TmsPricePackages");

            migrationBuilder.DropColumn(
                name: "DestinationTenantId",
                table: "TmsPricePackages");

            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "TmsPricePackages");

            migrationBuilder.AddColumn<decimal>(
                name: "DirectRequestCommission",
                table: "TmsPricePackages",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DirectRequestPrice",
                table: "TmsPricePackages",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DirectRequestTotalPrice",
                table: "TmsPricePackages",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "ShipperId",
                table: "TmsPricePackages",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TachyonManageCommission",
                table: "TmsPricePackages",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TachyonManagePrice",
                table: "TmsPricePackages",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TachyonManageTotalPrice",
                table: "TmsPricePackages",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<long>(
                name: "ReferencNumber",
                table: "ActorSubmitInvoices",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "InvoiceNumber",
                table: "ActorInvoices",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TmsPricePackages_ShipperId",
                table: "TmsPricePackages",
                column: "ShipperId");

            migrationBuilder.AddForeignKey(
                name: "FK_TmsPricePackages_AbpTenants_ShipperId",
                table: "TmsPricePackages",
                column: "ShipperId",
                principalTable: "AbpTenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
