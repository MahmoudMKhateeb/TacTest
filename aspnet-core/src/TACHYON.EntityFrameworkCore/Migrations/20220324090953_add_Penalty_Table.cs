using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class add_Penalty_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Penalties",
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
                    PenaltyName = table.Column<string>(nullable: true),
                    PenaltyDescrption = table.Column<string>(nullable: true),
                    Amount = table.Column<decimal>(nullable: false),
                    Type = table.Column<byte>(nullable: false),
                    TenantId = table.Column<int>(nullable: false),
                    TripId = table.Column<long>(nullable: true),
                    TripFKId = table.Column<int>(nullable: true),
                    PointId = table.Column<long>(nullable: true),
                    RoutPointFKId = table.Column<long>(nullable: true),
                    InvoiceId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Penalties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Penalties_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Penalties_RoutPoints_RoutPointFKId",
                        column: x => x.RoutPointFKId,
                        principalTable: "RoutPoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Penalties_AbpTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Penalties_ShippingRequestTrips_TripFKId",
                        column: x => x.TripFKId,
                        principalTable: "ShippingRequestTrips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });
            migrationBuilder.CreateIndex(
                name: "IX_Penalties_InvoiceId",
                table: "Penalties",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Penalties_RoutPointFKId",
                table: "Penalties",
                column: "RoutPointFKId");

            migrationBuilder.CreateIndex(
                name: "IX_Penalties_TenantId",
                table: "Penalties",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Penalties_TripFKId",
                table: "Penalties",
                column: "TripFKId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Penalties");
        }
    }
}
