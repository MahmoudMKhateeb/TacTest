using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace TACHYON.Migrations
{
    public partial class AddDocumentFiles_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DocumentFiles",
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
                    TenantId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    Extn = table.Column<string>(maxLength: 5, nullable: false),
                    BinaryObjectId = table.Column<Guid>(nullable: false),
                    ExpirationDate = table.Column<DateTime>(nullable: false),
                    IsAccepted = table.Column<string>(nullable: true),
                    DocumentTypeId = table.Column<long>(nullable: false),
                    TruckId = table.Column<Guid>(nullable: true),
                    TrailerId = table.Column<long>(nullable: true),
                    UserId = table.Column<long>(nullable: true),
                    RoutStepId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentFiles_DocumentTypes_DocumentTypeId",
                        column: x => x.DocumentTypeId,
                        principalTable: "DocumentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentFiles_RoutSteps_RoutStepId",
                        column: x => x.RoutStepId,
                        principalTable: "RoutSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocumentFiles_Trailers_TrailerId",
                        column: x => x.TrailerId,
                        principalTable: "Trailers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocumentFiles_Trucks_TruckId",
                        column: x => x.TruckId,
                        principalTable: "Trucks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocumentFiles_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentFiles_DocumentTypeId",
                table: "DocumentFiles",
                column: "DocumentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentFiles_RoutStepId",
                table: "DocumentFiles",
                column: "RoutStepId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentFiles_TenantId",
                table: "DocumentFiles",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentFiles_TrailerId",
                table: "DocumentFiles",
                column: "TrailerId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentFiles_TruckId",
                table: "DocumentFiles",
                column: "TruckId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentFiles_UserId",
                table: "DocumentFiles",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentFiles");
        }
    }
}