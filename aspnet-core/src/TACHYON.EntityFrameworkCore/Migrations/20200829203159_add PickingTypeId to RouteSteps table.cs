using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class addPickingTypeIdtoRouteStepstable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PickingTypeId",
                table: "RoutSteps",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_RoutSteps_PickingTypeId",
                table: "RoutSteps",
                column: "PickingTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoutSteps_PickingTypes_PickingTypeId",
                table: "RoutSteps",
                column: "PickingTypeId",
                principalTable: "PickingTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoutSteps_PickingTypes_PickingTypeId",
                table: "RoutSteps");

            migrationBuilder.DropIndex(
                name: "IX_RoutSteps_PickingTypeId",
                table: "RoutSteps");

            migrationBuilder.DropColumn(
                name: "PickingTypeId",
                table: "RoutSteps");
        }
    }
}
