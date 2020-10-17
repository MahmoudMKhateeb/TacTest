using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class addtruckcategoryfieldsandremovetrailerIdfromShippingRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_TrailerTypes_TrailerTypeId",
                table: "ShippingRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_TrucksTypes_TrucksTypeId",
                table: "ShippingRequests");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequests_TrailerTypeId",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "TrailerTypeId",
                table: "ShippingRequests");


            migrationBuilder.AlterColumn<long>(
                name: "TrucksTypeId",
                table: "ShippingRequests",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CapacityId",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TransportSubtypeId",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TransportTypeId",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TruckSubtypeId",
                table: "ShippingRequests",
                nullable: true);

           

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequests_CapacityId",
                table: "ShippingRequests",
                column: "CapacityId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequests_TransportSubtypeId",
                table: "ShippingRequests",
                column: "TransportSubtypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequests_TransportTypeId",
                table: "ShippingRequests",
                column: "TransportTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequests_TruckSubtypeId",
                table: "ShippingRequests",
                column: "TruckSubtypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequests_Capacities_CapacityId",
                table: "ShippingRequests",
                column: "CapacityId",
                principalTable: "Capacities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequests_TransportSubtypes_TransportSubtypeId",
                table: "ShippingRequests",
                column: "TransportSubtypeId",
                principalTable: "TransportSubtypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequests_TransportTypes_TransportTypeId",
                table: "ShippingRequests",
                column: "TransportTypeId",
                principalTable: "TransportTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequests_TruckSubtypes_TruckSubtypeId",
                table: "ShippingRequests",
                column: "TruckSubtypeId",
                principalTable: "TruckSubtypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequests_TrucksTypes_TrucksTypeId",
                table: "ShippingRequests",
                column: "TrucksTypeId",
                principalTable: "TrucksTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

           
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_Capacities_CapacityId",
                table: "ShippingRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_TransportSubtypes_TransportSubtypeId",
                table: "ShippingRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_TransportTypes_TransportTypeId",
                table: "ShippingRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_TruckSubtypes_TruckSubtypeId",
                table: "ShippingRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_TrucksTypes_TrucksTypeId",
                table: "ShippingRequests");

          

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequests_CapacityId",
                table: "ShippingRequests");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequests_TransportSubtypeId",
                table: "ShippingRequests");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequests_TransportTypeId",
                table: "ShippingRequests");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequests_TruckSubtypeId",
                table: "ShippingRequests");

            

            migrationBuilder.DropColumn(
                name: "CapacityId",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "TransportSubtypeId",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "TransportTypeId",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "TruckSubtypeId",
                table: "ShippingRequests");

            migrationBuilder.AlterColumn<long>(
                name: "TrucksTypeId",
                table: "ShippingRequests",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AddColumn<int>(
                name: "TrailerTypeId",
                table: "ShippingRequests",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequests_TrailerTypeId",
                table: "ShippingRequests",
                column: "TrailerTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequests_TrailerTypes_TrailerTypeId",
                table: "ShippingRequests",
                column: "TrailerTypeId",
                principalTable: "TrailerTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequests_TrucksTypes_TrucksTypeId",
                table: "ShippingRequests",
                column: "TrucksTypeId",
                principalTable: "TrucksTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
