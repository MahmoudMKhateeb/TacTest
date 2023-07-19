using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class add_report_parameters_with_definition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReportDefinitions_Reports_ReportTemplateId",
                table: "ReportDefinitions");

            migrationBuilder.DropTable(
                name: "Reports");

            migrationBuilder.CreateTable(
                name: "ReportParameterDefinitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ReportDefinitionId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportParameterDefinitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportParameterDefinitions_ReportDefinitions_ReportDefinitionId",
                        column: x => x.ReportDefinitionId,
                        principalTable: "ReportDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReportTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    Url = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Data = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReportParameters",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ReportParameterDefinitionId = table.Column<Guid>(nullable: false),
                    Value = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportParameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportParameters_ReportParameterDefinitions_ReportParameterDefinitionId",
                        column: x => x.ReportParameterDefinitionId,
                        principalTable: "ReportParameterDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReportParameterDefinitions_ReportDefinitionId",
                table: "ReportParameterDefinitions",
                column: "ReportDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportParameters_ReportParameterDefinitionId",
                table: "ReportParameters",
                column: "ReportParameterDefinitionId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReportDefinitions_ReportTemplates_ReportTemplateId",
                table: "ReportDefinitions",
                column: "ReportTemplateId",
                principalTable: "ReportTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReportDefinitions_ReportTemplates_ReportTemplateId",
                table: "ReportDefinitions");

            migrationBuilder.DropTable(
                name: "ReportParameters");

            migrationBuilder.DropTable(
                name: "ReportTemplates");

            migrationBuilder.DropTable(
                name: "ReportParameterDefinitions");

            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    Data = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_ReportDefinitions_Reports_ReportTemplateId",
                table: "ReportDefinitions",
                column: "ReportTemplateId",
                principalTable: "Reports",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
