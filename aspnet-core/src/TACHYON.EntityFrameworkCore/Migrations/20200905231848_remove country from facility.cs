using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class removecountryfromfacility : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Facilities_Counties_CountyId",
                table: "Facilities");

            migrationBuilder.DropIndex(
                name: "IX_Facilities_CountyId",
                table: "Facilities");

            migrationBuilder.DropColumn(
                name: "CountyId",
                table: "Facilities");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CountyId",
                table: "Facilities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Facilities_CountyId",
                table: "Facilities",
                column: "CountyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Facilities_Counties_CountyId",
                table: "Facilities",
                column: "CountyId",
                principalTable: "Counties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
