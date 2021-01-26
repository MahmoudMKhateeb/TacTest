using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class Added_PlateType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PlateTypeId",
                table: "Trucks",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PlateTypes",
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
                    DisplayName = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlateTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Trucks_PlateTypeId",
                table: "Trucks",
                column: "PlateTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Trucks_PlateTypes_PlateTypeId",
                table: "Trucks",
                column: "PlateTypeId",
                principalTable: "PlateTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trucks_PlateTypes_PlateTypeId",
                table: "Trucks");

            migrationBuilder.DropTable(
                name: "PlateTypes");

            migrationBuilder.DropIndex(
                name: "IX_Trucks_PlateTypeId",
                table: "Trucks");

            migrationBuilder.DropColumn(
                name: "PlateTypeId",
                table: "Trucks");
        }
    }
}
