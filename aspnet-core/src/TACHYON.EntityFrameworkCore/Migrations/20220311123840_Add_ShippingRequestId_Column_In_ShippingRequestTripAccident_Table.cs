using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class Add_ShippingRequestId_Column_In_ShippingRequestTripAccident_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ShippingRequestId",
                table: "ShippingRequestTripAccidents",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShippingRequestId",
                table: "ShippingRequestTripAccidents");
        }
    }
}
