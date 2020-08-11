using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class Add_IsBid_IsTachyonDeal_to_ShippingRequest_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBid",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTachyonDeal",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBid",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "IsTachyonDeal",
                table: "ShippingRequests");
        }
    }
}
