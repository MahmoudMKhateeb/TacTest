using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class Add_Columns_OtherTransportType_OtherTruckType_Truck_Tbl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OtherTransportTypeName",
                table: "Trucks",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherTrucksTypeName",
                table: "Trucks",
                nullable: true);

           
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
          
            migrationBuilder.DropColumn(
                name: "OtherTransportTypeName",
                table: "Trucks");

            migrationBuilder.DropColumn(
                name: "OtherTrucksTypeName",
                table: "Trucks");

        }
    }
}
