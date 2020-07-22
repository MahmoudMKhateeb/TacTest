using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace TACHYON.Migrations
{
    public partial class Add_Trailers_Tables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Trailers",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    TrailerCode = table.Column<string>(maxLength: 256, nullable: false),
                    PlateNumber = table.Column<string>(maxLength: 256, nullable: false),
                    Model = table.Column<string>(maxLength: 64, nullable: false),
                    Year = table.Column<string>(maxLength: 64, nullable: false),
                    Width = table.Column<int>(nullable: false),
                    Height = table.Column<int>(nullable: false),
                    Length = table.Column<int>(nullable: false),
                    IsLiftgate = table.Column<bool>(nullable: false),
                    IsReefer = table.Column<bool>(nullable: false),
                    IsVented = table.Column<bool>(nullable: false),
                    IsRollDoor = table.Column<bool>(nullable: false),
                    TrailerStatusId = table.Column<int>(nullable: false),
                    TrailerTypeId = table.Column<int>(nullable: false),
                    PayloadMaxWeightId = table.Column<int>(nullable: false),
                    HookedTruckId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trailers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Trailers_Trucks_HookedTruckId",
                        column: x => x.HookedTruckId,
                        principalTable: "Trucks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Trailers_PayloadMaxWeights_PayloadMaxWeightId",
                        column: x => x.PayloadMaxWeightId,
                        principalTable: "PayloadMaxWeights",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Trailers_TrailerStatuses_TrailerStatusId",
                        column: x => x.TrailerStatusId,
                        principalTable: "TrailerStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Trailers_TrailerTypes_TrailerTypeId",
                        column: x => x.TrailerTypeId,
                        principalTable: "TrailerTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Trailers_HookedTruckId",
                table: "Trailers",
                column: "HookedTruckId");

            migrationBuilder.CreateIndex(
                name: "IX_Trailers_PayloadMaxWeightId",
                table: "Trailers",
                column: "PayloadMaxWeightId");

            migrationBuilder.CreateIndex(
                name: "IX_Trailers_TrailerStatusId",
                table: "Trailers",
                column: "TrailerStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Trailers_TrailerTypeId",
                table: "Trailers",
                column: "TrailerTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Trailers");
        }
    }
}