using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class TerminologieEditionUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TerminologieEditions_AbpEditions_EditionId",
                table: "TerminologieEditions");

            migrationBuilder.AlterColumn<int>(
                name: "EditionId",
                table: "TerminologieEditions",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TerminologieEditions_AbpEditions_EditionId",
                table: "TerminologieEditions",
                column: "EditionId",
                principalTable: "AbpEditions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TerminologieEditions_AbpEditions_EditionId",
                table: "TerminologieEditions");

            migrationBuilder.AlterColumn<int>(
                name: "EditionId",
                table: "TerminologieEditions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_TerminologieEditions_AbpEditions_EditionId",
                table: "TerminologieEditions",
                column: "EditionId",
                principalTable: "AbpEditions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
