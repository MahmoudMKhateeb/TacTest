using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class SAB_directtrip : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Distance",
                table: "ShippingRequestTrips",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DriverWorkingHour",
                table: "ShippingRequestTrips",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LoadingType",
                table: "ShippingRequestTrips",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "ShippingRequestTrips",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SalesOfficeType",
                table: "ShippingRequestTrips",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "CityId",
                table: "Actors",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Actors_CityId",
                table: "Actors",
                column: "CityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Actors_Cities_CityId",
                table: "Actors",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Actors_Cities_CityId",
                table: "Actors");

            migrationBuilder.DropIndex(
                name: "IX_Actors_CityId",
                table: "Actors");

            migrationBuilder.DropColumn(
                name: "Distance",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "DriverWorkingHour",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "LoadingType",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "SalesOfficeType",
                table: "ShippingRequestTrips");

            migrationBuilder.AlterColumn<int>(
                name: "CityId",
                table: "Actors",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
