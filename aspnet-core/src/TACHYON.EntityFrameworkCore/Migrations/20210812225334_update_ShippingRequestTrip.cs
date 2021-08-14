using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class update_ShippingRequestTrip : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceiverEmailAddress",
                table: "RoutPoints");

            migrationBuilder.AlterColumn<string>(
                name: "Note",
                table: "ShippingRequestTrips",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(600)",
                oldMaxLength: 600,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Note",
                table: "ShippingRequestTrips",
                type: "nvarchar(600)",
                maxLength: 600,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 512,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReceiverEmailAddress",
                table: "RoutPoints",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
