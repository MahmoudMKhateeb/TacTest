using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace TACHYON.Migrations
{
    public partial class Add_GoodsDetails_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GoodsDetails",
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
                    TenantId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: false),
                    Description = table.Column<string>(maxLength: 256, nullable: true),
                    Quantity = table.Column<string>(maxLength: 128, nullable: true),
                    Weight = table.Column<string>(maxLength: 64, nullable: true),
                    Dimentions = table.Column<string>(maxLength: 128, nullable: true),
                    IsDangerousGood = table.Column<bool>(nullable: false),
                    DangerousGoodsCode = table.Column<string>(maxLength: 64, nullable: true),
                    GoodCategoryId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoodsDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GoodsDetails_GoodCategories_GoodCategoryId",
                        column: x => x.GoodCategoryId,
                        principalTable: "GoodCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GoodsDetails_GoodCategoryId",
                table: "GoodsDetails",
                column: "GoodCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsDetails_TenantId",
                table: "GoodsDetails",
                column: "TenantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GoodsDetails");
        }
    }
}