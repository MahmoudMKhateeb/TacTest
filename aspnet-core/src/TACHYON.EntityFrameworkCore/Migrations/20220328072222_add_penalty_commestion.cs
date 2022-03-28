using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class add_penalty_commestion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "Penalties");

            migrationBuilder.AddColumn<decimal>(
                name: "AmountPostCommestion",
                table: "Penalties",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountPreCommestion",
                table: "Penalties",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<byte>(
                name: "CommissionType",
                table: "Penalties",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<decimal>(
                name: "CommissionValue",
                table: "Penalties",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "SourceFeature",
                table: "Penalties",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                table: "Penalties",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "VatAmount",
                table: "Penalties",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "VatPostCommestion",
                table: "Penalties",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "VatPreCommestion",
                table: "Penalties",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountPostCommestion",
                table: "Penalties");

            migrationBuilder.DropColumn(
                name: "AmountPreCommestion",
                table: "Penalties");

            migrationBuilder.DropColumn(
                name: "CommissionType",
                table: "Penalties");

            migrationBuilder.DropColumn(
                name: "CommissionValue",
                table: "Penalties");

            migrationBuilder.DropColumn(
                name: "SourceFeature",
                table: "Penalties");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "Penalties");

            migrationBuilder.DropColumn(
                name: "VatAmount",
                table: "Penalties");

            migrationBuilder.DropColumn(
                name: "VatPostCommestion",
                table: "Penalties");

            migrationBuilder.DropColumn(
                name: "VatPreCommestion",
                table: "Penalties");

            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "Penalties",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
