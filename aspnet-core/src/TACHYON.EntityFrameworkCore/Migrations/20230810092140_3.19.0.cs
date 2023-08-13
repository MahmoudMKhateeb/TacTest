using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class _3190 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JsonDataSourceStorages",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConnectionName = table.Column<string>(nullable: true),
                    ConnectionValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JsonDataSourceStorages", x => x.Id);
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
                name: "ReportDefinitions",
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
                    DisplayName = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    ReportTemplateId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportDefinitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportDefinitions_ReportTemplates_ReportTemplateId",
                        column: x => x.ReportTemplateId,
                        principalTable: "ReportTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReportDefinitionPermissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TenantId = table.Column<int>(nullable: true),
                    EditionId = table.Column<int>(nullable: true),
                    ReportDefinitionId = table.Column<int>(nullable: false),
                    IsGranted = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportDefinitionPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportDefinitionPermissions_AbpEditions_EditionId",
                        column: x => x.EditionId,
                        principalTable: "AbpEditions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReportDefinitionPermissions_ReportDefinitions_ReportDefinitionId",
                        column: x => x.ReportDefinitionId,
                        principalTable: "ReportDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReportDefinitionPermissions_AbpTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReportParameterDefinitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ReportDefinitionId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
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
                name: "Reports",
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
                    DisplayName = table.Column<string>(nullable: true),
                    Format = table.Column<int>(nullable: false),
                    ReportDefinitionId = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false),
                    GeneratedFileId = table.Column<Guid>(nullable: true),
                    IsPublished = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reports_AppBinaryObjects_GeneratedFileId",
                        column: x => x.GeneratedFileId,
                        principalTable: "AppBinaryObjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reports_ReportDefinitions_ReportDefinitionId",
                        column: x => x.ReportDefinitionId,
                        principalTable: "ReportDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reports_AbpTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReportParameters",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ReportParameterDefinitionId = table.Column<Guid>(nullable: false),
                    Value = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ReportId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportParameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportParameters_Reports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "Reports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReportParameters_ReportParameterDefinitions_ReportParameterDefinitionId",
                        column: x => x.ReportParameterDefinitionId,
                        principalTable: "ReportParameterDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReportPermissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RoleId = table.Column<int>(nullable: true),
                    UserId = table.Column<long>(nullable: true),
                    ReportId = table.Column<Guid>(nullable: false),
                    IsGranted = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportPermissions_Reports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "Reports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReportPermissions_AbpRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AbpRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReportPermissions_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReportDefinitionPermissions_EditionId",
                table: "ReportDefinitionPermissions",
                column: "EditionId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportDefinitionPermissions_ReportDefinitionId",
                table: "ReportDefinitionPermissions",
                column: "ReportDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportDefinitionPermissions_TenantId",
                table: "ReportDefinitionPermissions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportDefinitions_ReportTemplateId",
                table: "ReportDefinitions",
                column: "ReportTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportParameterDefinitions_ReportDefinitionId",
                table: "ReportParameterDefinitions",
                column: "ReportDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportParameters_ReportId",
                table: "ReportParameters",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportParameters_ReportParameterDefinitionId",
                table: "ReportParameters",
                column: "ReportParameterDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportPermissions_ReportId",
                table: "ReportPermissions",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportPermissions_RoleId",
                table: "ReportPermissions",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportPermissions_UserId",
                table: "ReportPermissions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_GeneratedFileId",
                table: "Reports",
                column: "GeneratedFileId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_ReportDefinitionId",
                table: "Reports",
                column: "ReportDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_TenantId",
                table: "Reports",
                column: "TenantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JsonDataSourceStorages");

            migrationBuilder.DropTable(
                name: "ReportDefinitionPermissions");

            migrationBuilder.DropTable(
                name: "ReportParameters");

            migrationBuilder.DropTable(
                name: "ReportPermissions");

            migrationBuilder.DropTable(
                name: "ReportParameterDefinitions");

            migrationBuilder.DropTable(
                name: "Reports");

            migrationBuilder.DropTable(
                name: "ReportDefinitions");

            migrationBuilder.DropTable(
                name: "ReportTemplates");
        }
    }
}
