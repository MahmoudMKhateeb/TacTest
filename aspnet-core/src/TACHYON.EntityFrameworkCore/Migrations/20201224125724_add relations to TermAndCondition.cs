using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class addrelationstoTermAndCondition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TermAndConditionTranslations_TermAndConditions_CoreId",
                table: "TermAndConditionTranslations");

            migrationBuilder.DropColumn(
                name: "Content",
                table: "TermAndConditions");

            migrationBuilder.AlterColumn<int>(
                name: "CoreId",
                table: "TermAndConditionTranslations",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TermAndConditions_EditionId",
                table: "TermAndConditions",
                column: "EditionId");

            migrationBuilder.AddForeignKey(
                name: "FK_TermAndConditions_AbpEditions_EditionId",
                table: "TermAndConditions",
                column: "EditionId",
                principalTable: "AbpEditions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TermAndConditionTranslations_TermAndConditions_CoreId",
                table: "TermAndConditionTranslations",
                column: "CoreId",
                principalTable: "TermAndConditions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TermAndConditions_AbpEditions_EditionId",
                table: "TermAndConditions");

            migrationBuilder.DropForeignKey(
                name: "FK_TermAndConditionTranslations_TermAndConditions_CoreId",
                table: "TermAndConditionTranslations");

            migrationBuilder.DropIndex(
                name: "IX_TermAndConditions_EditionId",
                table: "TermAndConditions");

            migrationBuilder.AlterColumn<int>(
                name: "CoreId",
                table: "TermAndConditionTranslations",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "TermAndConditions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_TermAndConditionTranslations_TermAndConditions_CoreId",
                table: "TermAndConditionTranslations",
                column: "CoreId",
                principalTable: "TermAndConditions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
