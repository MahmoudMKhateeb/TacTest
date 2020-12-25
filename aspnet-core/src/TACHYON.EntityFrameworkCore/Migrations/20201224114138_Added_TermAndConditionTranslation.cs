using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class Added_TermAndConditionTranslation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TermAndConditions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    Title = table.Column<string>(maxLength: 256, nullable: false),
                    Content = table.Column<string>(nullable: false),
                    Version = table.Column<double>(nullable: false),
                    EditionId = table.Column<int>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TermAndConditions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TermAndConditionTranslations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(nullable: false),
                    Language = table.Column<string>(maxLength: 50, nullable: false),
                    CoreId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TermAndConditionTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TermAndConditionTranslations_TermAndConditions_CoreId",
                        column: x => x.CoreId,
                        principalTable: "TermAndConditions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TermAndConditions_TenantId",
                table: "TermAndConditions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TermAndConditionTranslations_CoreId",
                table: "TermAndConditionTranslations",
                column: "CoreId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TermAndConditionTranslations");

            migrationBuilder.DropTable(
                name: "TermAndConditions");
        }
    }
}
