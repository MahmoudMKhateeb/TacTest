using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class Add_container_returnDate_to_the_trip : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ContainerReturnDate",
                table: "ShippingRequestTrips",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContainerReturnDate",
                table: "ShippingRequestTrips");
        }
    }
}
