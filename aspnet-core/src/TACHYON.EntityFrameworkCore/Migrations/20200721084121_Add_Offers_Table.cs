using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace TACHYON.Migrations
{
    public partial class Add_Offers_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Offers",
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
                    TenantId = table.Column<int>(nullable: false),
                    DisplayName = table.Column<string>(maxLength: 256, nullable: true),
                    Description = table.Column<string>(maxLength: 256, nullable: true),
                    Price = table.Column<decimal>(nullable: false),
                    TrucksTypeId = table.Column<Guid>(nullable: false),
                    TrailerTypeId = table.Column<int>(nullable: false),
                    GoodCategoryId = table.Column<int>(nullable: true),
                    RouteId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Offers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Offers_GoodCategories_GoodCategoryId",
                        column: x => x.GoodCategoryId,
                        principalTable: "GoodCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Offers_Routes_RouteId",
                        column: x => x.RouteId,
                        principalTable: "Routes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Offers_TrailerTypes_TrailerTypeId",
                        column: x => x.TrailerTypeId,
                        principalTable: "TrailerTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Offers_TrucksTypes_TrucksTypeId",
                        column: x => x.TrucksTypeId,
                        principalTable: "TrucksTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Offers_GoodCategoryId",
                table: "Offers",
                column: "GoodCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Offers_RouteId",
                table: "Offers",
                column: "RouteId");

            migrationBuilder.CreateIndex(
                name: "IX_Offers_TenantId",
                table: "Offers",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Offers_TrailerTypeId",
                table: "Offers",
                column: "TrailerTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Offers_TrucksTypeId",
                table: "Offers",
                column: "TrucksTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Offers");
        }
    }
}