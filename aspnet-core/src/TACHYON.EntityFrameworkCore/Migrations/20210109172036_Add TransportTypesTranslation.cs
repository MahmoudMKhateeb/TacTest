using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class AddTransportTypesTranslation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "TransportTypes");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "TransportTypes",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "TransportTypesTranslations",
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
                    DisplayName = table.Column<string>(maxLength: 256, nullable: false),
                    Language = table.Column<string>(maxLength: 32, nullable: false),
                    CoreId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransportTypesTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransportTypesTranslations_TransportTypes_CoreId",
                        column: x => x.CoreId,
                        principalTable: "TransportTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });


            migrationBuilder.CreateIndex(
                name: "IX_TransportTypesTranslations_CoreId",
                table: "TransportTypesTranslations",
                column: "CoreId");


        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {


            migrationBuilder.DropColumn(
                name: "Name",
                table: "TransportTypes");

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "TransportTypes",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

        }
    }
}
