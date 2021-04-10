using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

namespace TACHYON.Migrations
{
    public partial class ShippingRequestTripTransition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoutePointTransitions");

            migrationBuilder.CreateTable(
                name: "ShippingRequestTripTransitions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FromPointId = table.Column<long>(nullable: true),
                    FromLocation = table.Column<Point>(nullable: true),
                    ToPointId = table.Column<long>(nullable: false),
                    ToLocation = table.Column<Point>(nullable: true),
                    IsComplete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingRequestTripTransitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingRequestTripTransitions_RoutPoints_FromPointId",
                        column: x => x.FromPointId,
                        principalTable: "RoutPoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShippingRequestTripTransitions_RoutPoints_ToPointId",
                        column: x => x.ToPointId,
                        principalTable: "RoutPoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTripTransitions_FromPointId",
                table: "ShippingRequestTripTransitions",
                column: "FromPointId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTripTransitions_ToPointId",
                table: "ShippingRequestTripTransitions",
                column: "ToPointId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShippingRequestTripTransitions");

            migrationBuilder.CreateTable(
                name: "RoutePointTransitions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FromPointId = table.Column<long>(type: "bigint", nullable: false),
                    ToPointId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoutePointTransitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoutePointTransitions_RoutPoints_FromPointId",
                        column: x => x.FromPointId,
                        principalTable: "RoutPoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoutePointTransitions_RoutPoints_ToPointId",
                        column: x => x.ToPointId,
                        principalTable: "RoutPoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoutePointTransitions_FromPointId",
                table: "RoutePointTransitions",
                column: "FromPointId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutePointTransitions_ToPointId",
                table: "RoutePointTransitions",
                column: "ToPointId");
        }
    }
}
