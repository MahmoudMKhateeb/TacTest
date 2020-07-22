using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class Add_RentPrice_and_RentDuration_To_Truck_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RentDuration",
                table: "Trucks",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RentPrice",
                table: "Trucks",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RentDuration",
                table: "Trucks");

            migrationBuilder.DropColumn(
                name: "RentPrice",
                table: "Trucks");
        }
    }
}