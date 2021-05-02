using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class AppLocalization : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "ShippingRequestTripRejectReasons");

            migrationBuilder.CreateTable(
                name: "AppLocalizations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MasterKey = table.Column<string>(nullable: false),
                    MasterValue = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppLocalizations", x => x.Id);
                });



            migrationBuilder.CreateTable(
                name: "AppLocalizationTranslations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(nullable: false),
                    CoreId = table.Column<int>(nullable: false),
                    Language = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppLocalizationTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppLocalizationTranslations_AppLocalizations_CoreId",
                        column: x => x.CoreId,
                        principalTable: "AppLocalizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppLocalizationTranslations_CoreId",
                table: "AppLocalizationTranslations",
                column: "CoreId");


        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppLocalizationTranslations");


            migrationBuilder.DropTable(
                name: "AppLocalizations");

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "ShippingRequestTripRejectReasons",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }
    }
}
