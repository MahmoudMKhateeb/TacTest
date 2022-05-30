using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace TACHYON.Migrations
{
    public partial class add_normalPricePackage_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "NormalPricePackages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    TenantId = table.Column<int>(nullable: false),
                    TransportTypeId = table.Column<int>(nullable: false),
                    TrucksTypeId = table.Column<long>(nullable: false),
                    DirectRequestPrice = table.Column<float>(nullable: false),
                    MarcketPlaceRequestPrice = table.Column<float>(nullable: false),
                    TachyonMSRequestPrice = table.Column<float>(nullable: false),
                    PricePerExtraDrop = table.Column<float>(nullable: true),
                    IsMultiDrop = table.Column<bool>(nullable: false),
                    OriginCityId = table.Column<int>(nullable: true),
                    DestinationCityId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NormalPricePackages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NormalPricePackages_Cities_DestinationCityId",
                        column: x => x.DestinationCityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NormalPricePackages_Cities_OriginCityId",
                        column: x => x.OriginCityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NormalPricePackages_AbpTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NormalPricePackages_TransportTypes_TransportTypeId",
                        column: x => x.TransportTypeId,
                        principalTable: "TransportTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NormalPricePackages_TrucksTypes_TrucksTypeId",
                        column: x => x.TrucksTypeId,
                        principalTable: "TrucksTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NormalPricePackages_DestinationCityId",
                table: "NormalPricePackages",
                column: "DestinationCityId");

            migrationBuilder.CreateIndex(
                name: "IX_NormalPricePackages_OriginCityId",
                table: "NormalPricePackages",
                column: "OriginCityId");

            migrationBuilder.CreateIndex(
                name: "IX_NormalPricePackages_TenantId",
                table: "NormalPricePackages",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_NormalPricePackages_TransportTypeId",
                table: "NormalPricePackages",
                column: "TransportTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_NormalPricePackages_TrucksTypeId",
                table: "NormalPricePackages",
                column: "TrucksTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NormalPricePackages");

        }
    }
}