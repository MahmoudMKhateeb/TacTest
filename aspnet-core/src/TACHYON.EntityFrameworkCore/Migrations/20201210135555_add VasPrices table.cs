using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class addVasPricestable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VasPrices",
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
                    Price = table.Column<double>(nullable: true),
                    MaxAmount = table.Column<int>(nullable: true),
                    MaxCount = table.Column<int>(nullable: true),
                    VasId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VasPrices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VasPrices_Vases_VasId",
                        column: x => x.VasId,
                        principalTable: "Vases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VasPrices_TenantId",
                table: "VasPrices",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_VasPrices_VasId",
                table: "VasPrices",
                column: "VasId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VasPrices");
        }
    }
}
