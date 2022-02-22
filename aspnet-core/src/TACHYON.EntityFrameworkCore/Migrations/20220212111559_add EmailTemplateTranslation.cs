using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class addEmailTemplateTranslation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropColumn(
                name: "Html",
                table: "EmailTemplates");


            migrationBuilder.CreateTable(
                name: "EmailTemplateTranslations",
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
                    TranslatedContent = table.Column<string>(nullable: true),
                    Language = table.Column<string>(nullable: false),
                    CoreId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailTemplateTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailTemplateTranslations_EmailTemplates_CoreId",
                        column: x => x.CoreId,
                        principalTable: "EmailTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmailTemplateTranslations_CoreId",
                table: "EmailTemplateTranslations",
                column: "CoreId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailTemplateTranslations");

          
            migrationBuilder.AddColumn<string>(
                name: "Html",
                table: "EmailTemplates",
                type: "nvarchar(max)",
                nullable: true);

          

           
        }
    }
}
