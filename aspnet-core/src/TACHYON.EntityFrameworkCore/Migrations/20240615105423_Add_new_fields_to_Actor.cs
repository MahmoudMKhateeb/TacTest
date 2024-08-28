using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class Add_new_fields_to_Actor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountType",
                table: "Actors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BuildingCode",
                table: "Actors",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CityId",
                table: "Actors",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CustomerGroup",
                table: "Actors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "District",
                table: "Actors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Division",
                table: "Actors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Actors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Actors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "Actors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Reconsaccoun",
                table: "Actors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Region",
                table: "Actors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SalesGroup",
                table: "Actors",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SalesOfficeType",
                table: "Actors",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TrasportationZone",
                table: "Actors",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSAB",
                table: "AbpTenants",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountType",
                table: "Actors");

            migrationBuilder.DropColumn(
                name: "BuildingCode",
                table: "Actors");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "Actors");

            migrationBuilder.DropColumn(
                name: "CustomerGroup",
                table: "Actors");

            migrationBuilder.DropColumn(
                name: "District",
                table: "Actors");

            migrationBuilder.DropColumn(
                name: "Division",
                table: "Actors");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Actors");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Actors");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "Actors");

            migrationBuilder.DropColumn(
                name: "Reconsaccoun",
                table: "Actors");

            migrationBuilder.DropColumn(
                name: "Region",
                table: "Actors");

            migrationBuilder.DropColumn(
                name: "SalesGroup",
                table: "Actors");

            migrationBuilder.DropColumn(
                name: "SalesOfficeType",
                table: "Actors");

            migrationBuilder.DropColumn(
                name: "TrasportationZone",
                table: "Actors");

            migrationBuilder.DropColumn(
                name: "IsSAB",
                table: "AbpTenants");
        }
    }
}
