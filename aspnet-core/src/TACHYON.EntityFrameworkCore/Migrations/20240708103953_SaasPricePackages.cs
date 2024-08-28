using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class SaasPricePackages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SaasPricePackages",
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
                    PricePackageReference = table.Column<string>(nullable: true),
                    DisplayName = table.Column<string>(nullable: false),
                    TenantId = table.Column<int>(nullable: false),
                    TransportTypeId = table.Column<int>(nullable: false),
                    TruckTypeId = table.Column<long>(nullable: false),
                    OriginCityId = table.Column<int>(nullable: true),
                    DestinationCityId = table.Column<int>(nullable: true),
                    ActorShipperId = table.Column<int>(nullable: true),
                    ActorShipperPrice = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaasPricePackages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaasPricePackages_Actors_ActorShipperId",
                        column: x => x.ActorShipperId,
                        principalTable: "Actors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaasPricePackages_Cities_DestinationCityId",
                        column: x => x.DestinationCityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaasPricePackages_Cities_OriginCityId",
                        column: x => x.OriginCityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaasPricePackages_TransportTypes_TransportTypeId",
                        column: x => x.TransportTypeId,
                        principalTable: "TransportTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SaasPricePackages_TrucksTypes_TruckTypeId",
                        column: x => x.TruckTypeId,
                        principalTable: "TrucksTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SaasPricePackages_ActorShipperId",
                table: "SaasPricePackages",
                column: "ActorShipperId");

            migrationBuilder.CreateIndex(
                name: "IX_SaasPricePackages_DestinationCityId",
                table: "SaasPricePackages",
                column: "DestinationCityId");

            migrationBuilder.CreateIndex(
                name: "IX_SaasPricePackages_OriginCityId",
                table: "SaasPricePackages",
                column: "OriginCityId");

            migrationBuilder.CreateIndex(
                name: "IX_SaasPricePackages_TransportTypeId",
                table: "SaasPricePackages",
                column: "TransportTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SaasPricePackages_TruckTypeId",
                table: "SaasPricePackages",
                column: "TruckTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SaasPricePackages");
        }
    }
}
