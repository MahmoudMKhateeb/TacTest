using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class RatingSetRateToDecimal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Rate",
                table: "RatingLogs",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Rate",
                table: "RatingLogs",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal));
        }
    }
}
