using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace TACHYON.Migrations
{
    public partial class Add_Trucks_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Trucks",
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
                    TenantId = table.Column<int>(nullable: false),
                    PlateNumber = table.Column<string>(maxLength: 64, nullable: false),
                    ModelName = table.Column<string>(maxLength: 64, nullable: false),
                    ModelYear = table.Column<string>(maxLength: 64, nullable: false),
                    LicenseNumber = table.Column<string>(maxLength: 256, nullable: false),
                    LicenseExpirationDate = table.Column<DateTime>(nullable: false),
                    IsAttachable = table.Column<bool>(nullable: false),
                    Note = table.Column<string>(maxLength: 256, nullable: true),
                    TrucksTypeId = table.Column<Guid>(nullable: false),
                    TruckStatusId = table.Column<Guid>(nullable: false),
                    Driver1UserId = table.Column<long>(nullable: true),
                    Driver2UserId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trucks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Trucks_AbpUsers_Driver1UserId",
                        column: x => x.Driver1UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Trucks_AbpUsers_Driver2UserId",
                        column: x => x.Driver2UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Trucks_TruckStatuses_TruckStatusId",
                        column: x => x.TruckStatusId,
                        principalTable: "TruckStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Trucks_TrucksTypes_TrucksTypeId",
                        column: x => x.TrucksTypeId,
                        principalTable: "TrucksTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Trucks_Driver1UserId",
                table: "Trucks",
                column: "Driver1UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Trucks_Driver2UserId",
                table: "Trucks",
                column: "Driver2UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Trucks_TenantId",
                table: "Trucks",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Trucks_TruckStatusId",
                table: "Trucks",
                column: "TruckStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Trucks_TrucksTypeId",
                table: "Trucks",
                column: "TrucksTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Trucks");
        }
    }
}