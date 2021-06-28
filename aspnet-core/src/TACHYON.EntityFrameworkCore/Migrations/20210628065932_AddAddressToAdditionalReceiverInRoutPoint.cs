using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class AddAddressToAdditionalReceiverInRoutPoint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoutePointReceiverReceiveShipmentCodes");

            migrationBuilder.AddColumn<string>(
                name: "ReceiverAddress",
                table: "RoutPoints",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceiverAddress",
                table: "RoutPoints");

            migrationBuilder.CreateTable(
                name: "RoutePointReceiverReceiveShipmentCodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PointId = table.Column<long>(type: "bigint", nullable: true),
                    ReceiverPhone = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoutePointReceiverReceiveShipmentCodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoutePointReceiverReceiveShipmentCodes_RoutPoints_PointId",
                        column: x => x.PointId,
                        principalTable: "RoutPoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoutePointReceiverReceiveShipmentCodes_PointId",
                table: "RoutePointReceiverReceiveShipmentCodes",
                column: "PointId");
        }
    }
}
