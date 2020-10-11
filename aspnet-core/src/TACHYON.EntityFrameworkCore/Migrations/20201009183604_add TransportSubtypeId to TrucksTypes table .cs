using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class addTransportSubtypeIdtoTrucksTypestable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TransportSubtypeId",
                table: "TrucksTypes",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrucksTypes_TransportSubtypeId",
                table: "TrucksTypes",
                column: "TransportSubtypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrucksTypes_TransportSubtypes_TransportSubtypeId",
                table: "TrucksTypes",
                column: "TransportSubtypeId",
                principalTable: "TransportSubtypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrucksTypes_TransportSubtypes_TransportSubtypeId",
                table: "TrucksTypes");

            migrationBuilder.DropIndex(
                name: "IX_TrucksTypes_TransportSubtypeId",
                table: "TrucksTypes");

            migrationBuilder.DropColumn(
                name: "TransportSubtypeId",
                table: "TrucksTypes");
        }
    }
}
