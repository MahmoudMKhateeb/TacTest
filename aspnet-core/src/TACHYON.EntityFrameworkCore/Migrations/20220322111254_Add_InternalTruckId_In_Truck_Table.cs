using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace TACHYON.Migrations
{
    public partial class Add_InternalTruckId_In_Truck_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<string>(
                name: "InternalTruckId",
                table: "Trucks",
                maxLength: 10,
                nullable: true);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropColumn(
                name: "InternalTruckId",
                table: "Trucks");

        }
    }
}