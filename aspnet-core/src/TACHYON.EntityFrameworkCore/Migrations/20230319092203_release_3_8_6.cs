using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class release_3_8_6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "PricePackageId",
                table: "ServiceAreas",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OriginCountryId",
                table: "PricePackages",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProjectName",
                table: "PricePackages",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ScopeOfWork",
                table: "PricePackages",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceAreas_PricePackageId",
                table: "ServiceAreas",
                column: "PricePackageId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceAreas_PricePackages_PricePackageId",
                table: "ServiceAreas",
                column: "PricePackageId",
                principalTable: "PricePackages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceAreas_PricePackages_PricePackageId",
                table: "ServiceAreas");

            migrationBuilder.DropIndex(
                name: "IX_ServiceAreas_PricePackageId",
                table: "ServiceAreas");

            migrationBuilder.DropColumn(
                name: "PricePackageId",
                table: "ServiceAreas");

            migrationBuilder.DropColumn(
                name: "OriginCountryId",
                table: "PricePackages");

            migrationBuilder.DropColumn(
                name: "ProjectName",
                table: "PricePackages");

            migrationBuilder.DropColumn(
                name: "ScopeOfWork",
                table: "PricePackages");
        }
    }
}
