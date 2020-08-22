using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class changeTrucksTypesIdtolong : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "TrucksTypes",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<long>(
                name: "TrucksTypeId",
                table: "Trucks",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<long>(
                name: "TrucksTypeId",
                table: "ShippingRequests",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "TrucksTypeId",
                table: "Offers",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TrucksTypes",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(long))
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<Guid>(
                name: "TrucksTypeId",
                table: "Trucks",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<Guid>(
                name: "TrucksTypeId",
                table: "ShippingRequests",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "TrucksTypeId",
                table: "Offers",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(long));
        }
    }
}
