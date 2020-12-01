using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class add_CloseDate_ShippingRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.AddColumn<DateTime>(
                name: "CloseBidDate",
                table: "ShippingRequests",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CloseBidDate",
                table: "ShippingRequests");

           
        }
    }
}
