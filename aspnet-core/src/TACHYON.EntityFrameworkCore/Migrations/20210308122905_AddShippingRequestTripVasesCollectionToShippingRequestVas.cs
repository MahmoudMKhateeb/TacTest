using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class AddShippingRequestTripVasesCollectionToShippingRequestVas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShippingRequestTripVases",
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
                    ShippingRequestVasId = table.Column<long>(nullable: false),
                    ShippingRequestVasFkId = table.Column<long>(nullable: true),
                    ShippingRequestTripId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingRequestTripVases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingRequestTripVases_ShippingRequestTrips_ShippingRequestTripId",
                        column: x => x.ShippingRequestTripId,
                        principalTable: "ShippingRequestTrips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShippingRequestTripVases_ShippingRequestTripVases_ShippingRequestVasFkId",
                        column: x => x.ShippingRequestVasFkId,
                        principalTable: "ShippingRequestTripVases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShippingRequestTripVases_ShippingRequestVases_ShippingRequestVasId",
                        column: x => x.ShippingRequestVasId,
                        principalTable: "ShippingRequestVases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTripVases_ShippingRequestTripId",
                table: "ShippingRequestTripVases",
                column: "ShippingRequestTripId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTripVases_ShippingRequestVasFkId",
                table: "ShippingRequestTripVases",
                column: "ShippingRequestVasFkId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTripVases_ShippingRequestVasId",
                table: "ShippingRequestTripVases",
                column: "ShippingRequestVasId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShippingRequestTripVases");
        }
    }
}
