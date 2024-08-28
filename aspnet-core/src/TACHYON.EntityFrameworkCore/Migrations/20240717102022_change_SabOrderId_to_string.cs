using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class change_SabOrderId_to_string : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SabOrderId",
                table: "ShippingRequestTrips",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "SabOrderId",
                table: "ShippingRequestTrips",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
