using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class addTruckCategoriesfieldstoTruckstable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CapacityId",
                table: "Trucks",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TransportSubtypeId",
                table: "Trucks",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TransportTypeId",
                table: "Trucks",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TruckSubtypeId",
                table: "Trucks",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Trucks_CapacityId",
                table: "Trucks",
                column: "CapacityId");

            migrationBuilder.CreateIndex(
                name: "IX_Trucks_TransportSubtypeId",
                table: "Trucks",
                column: "TransportSubtypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Trucks_TransportTypeId",
                table: "Trucks",
                column: "TransportTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Trucks_TruckSubtypeId",
                table: "Trucks",
                column: "TruckSubtypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Trucks_Capacities_CapacityId",
                table: "Trucks",
                column: "CapacityId",
                principalTable: "Capacities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Trucks_TransportSubtypes_TransportSubtypeId",
                table: "Trucks",
                column: "TransportSubtypeId",
                principalTable: "TransportSubtypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Trucks_TransportTypes_TransportTypeId",
                table: "Trucks",
                column: "TransportTypeId",
                principalTable: "TransportTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Trucks_TruckSubtypes_TruckSubtypeId",
                table: "Trucks",
                column: "TruckSubtypeId",
                principalTable: "TruckSubtypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trucks_Capacities_CapacityId",
                table: "Trucks");

            migrationBuilder.DropForeignKey(
                name: "FK_Trucks_TransportSubtypes_TransportSubtypeId",
                table: "Trucks");

            migrationBuilder.DropForeignKey(
                name: "FK_Trucks_TransportTypes_TransportTypeId",
                table: "Trucks");

            migrationBuilder.DropForeignKey(
                name: "FK_Trucks_TruckSubtypes_TruckSubtypeId",
                table: "Trucks");

            migrationBuilder.DropIndex(
                name: "IX_Trucks_CapacityId",
                table: "Trucks");

            migrationBuilder.DropIndex(
                name: "IX_Trucks_TransportSubtypeId",
                table: "Trucks");

            migrationBuilder.DropIndex(
                name: "IX_Trucks_TransportTypeId",
                table: "Trucks");

            migrationBuilder.DropIndex(
                name: "IX_Trucks_TruckSubtypeId",
                table: "Trucks");

            migrationBuilder.DropColumn(
                name: "CapacityId",
                table: "Trucks");

            migrationBuilder.DropColumn(
                name: "TransportSubtypeId",
                table: "Trucks");

            migrationBuilder.DropColumn(
                name: "TransportTypeId",
                table: "Trucks");

            migrationBuilder.DropColumn(
                name: "TruckSubtypeId",
                table: "Trucks");
        }
    }
}
