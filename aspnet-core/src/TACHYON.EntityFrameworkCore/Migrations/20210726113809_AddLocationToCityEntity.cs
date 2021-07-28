using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

namespace TACHYON.Migrations
{
    public partial class AddLocationToCityEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Cities");

            migrationBuilder.AddColumn<Point>(
                name: "Location",
                table: "Cities",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTypes_DocumentRelatedWithId",
                table: "DocumentTypes",
                column: "DocumentRelatedWithId");

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentTypes_AbpTenants_DocumentRelatedWithId",
                table: "DocumentTypes",
                column: "DocumentRelatedWithId",
                principalTable: "AbpTenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentTypes_AbpTenants_DocumentRelatedWithId",
                table: "DocumentTypes");

            migrationBuilder.DropIndex(
                name: "IX_DocumentTypes_DocumentRelatedWithId",
                table: "DocumentTypes");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Cities");

            migrationBuilder.AddColumn<string>(
                name: "Latitude",
                table: "Cities",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Longitude",
                table: "Cities",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);
        }
    }
}
