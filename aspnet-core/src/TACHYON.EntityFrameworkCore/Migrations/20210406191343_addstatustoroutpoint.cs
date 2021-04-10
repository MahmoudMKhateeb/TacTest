using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class addstatustoroutpoint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "Status",
                table: "RoutPoints",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.CreateTable(
                name: "RoutPointStatusTransitions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PointId = table.Column<long>(nullable: false),
                    Status = table.Column<byte>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoutPointStatusTransitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoutPointStatusTransitions_RoutPoints_PointId",
                        column: x => x.PointId,
                        principalTable: "RoutPoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoutPointStatusTransitions_PointId",
                table: "RoutPointStatusTransitions",
                column: "PointId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoutPointStatusTransitions");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "RoutPoints");
        }
    }
}
