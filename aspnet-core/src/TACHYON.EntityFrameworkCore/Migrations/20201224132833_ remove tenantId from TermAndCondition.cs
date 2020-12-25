using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class removetenantIdfromTermAndCondition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TermAndConditions_TenantId",
                table: "TermAndConditions");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "TermAndConditions");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "TermAndConditions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TermAndConditions_TenantId",
                table: "TermAndConditions",
                column: "TenantId");
        }
    }
}
